using System.Diagnostics;
using System.Text.Json;

using Ecomm.Messaging;
using Ecomm.Payments.Application.IntegrationEvents;
using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;
using Ecomm.Payments.Domain.Primitives;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Payments.Infrastructure.Persistence;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public DbSet<Payment> Payments { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
        modelBuilder.AddMessagingEntities();
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var payments = ChangeTracker.Entries<Payment>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToArray();

        foreach (var payment in payments)
            AddResultOutboxMessage(payment);

        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var payment in payments)
            payment.ClearDomainEvents();
        return result;
    }

    private void AddResultOutboxMessage(Payment payment)
    {
        var now = timeProvider.GetUtcNow();
        var id = Guid.NewGuid();
        var correlationId = Activity.Current?.TraceId.ToString();
        var causationId = Activity.Current?.ParentSpanId.ToString();
        object data;
        string type;

        if (payment.Status == PaymentStatus.Approved)
        {
            type = "payments.payment-approved.v1";
            data = new PaymentApprovedIntegrationEvent(
                payment.OrderId, payment.Id, payment.ExternalPaymentId!);
        }
        else if (payment.Status == PaymentStatus.Rejected)
        {
            type = "payments.payment-rejected.v1";
            data = new PaymentRejectedIntegrationEvent(
                payment.OrderId, payment.Id, payment.ExternalPaymentId, payment.RejectionReason!);
        }
        else
        {
            return;
        }

        var envelope = new EventEnvelope<object>(id, type, 1, now, correlationId, causationId, data);
        OutboxMessages.Add(new OutboxMessage
        {
            Id = id,
            Type = type,
            Version = 1,
            Destination = "payments-events",
            Payload = JsonSerializer.Serialize(envelope, SerializerOptions),
            OccurredAtUtc = now,
            NextAttemptAtUtc = now,
            CorrelationId = correlationId,
            CausationId = causationId,
        });
    }
}
