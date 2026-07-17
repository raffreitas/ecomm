using System.Diagnostics;
using System.Text.Json;

using Ecomm.Messaging;
using Ecomm.Orders.Application.IntegrationEvents;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Events;
using Ecomm.Orders.Domain.Primitives;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Infrastructure.Persistence;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        modelBuilder.AddMessagingEntities();
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var trackedEntities = ChangeTracker.Entries<Entity>().Select(entry => entry.Entity).ToArray();
        var domainEvents = trackedEntities.SelectMany(entity => entity.DomainEvents).ToArray();

        foreach (var domainEvent in domainEvents.OfType<OrderCreatedDomainEvent>())
            AddOrderCreatedOutboxMessage(domainEvent);

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in trackedEntities)
            entity.ClearDomainEvents();

        return result;
    }

    private void AddOrderCreatedOutboxMessage(OrderCreatedDomainEvent domainEvent)
    {
        var integrationEvent = new OrderCreatedIntegrationEvent(
            domainEvent.Id,
            domainEvent.CustomerName,
            domainEvent.CustomerDocument,
            domainEvent.CardHash,
            domainEvent.Total);
        var now = timeProvider.GetUtcNow();
        var id = Guid.NewGuid();
        var correlationId = Activity.Current?.TraceId.ToString();
        var causationId = Activity.Current?.ParentSpanId.ToString();
        var envelope = new EventEnvelope<OrderCreatedIntegrationEvent>(
            id, "orders.order-created.v1", 1, now, correlationId, causationId, integrationEvent);

        OutboxMessages.Add(new OutboxMessage
        {
            Id = id,
            Type = envelope.Type,
            Version = envelope.Version,
            Destination = "orders-events",
            Payload = JsonSerializer.Serialize(envelope, SerializerOptions),
            OccurredAtUtc = now,
            NextAttemptAtUtc = now,
            CorrelationId = correlationId,
            CausationId = causationId,
        });
    }
}
