using System.Diagnostics;
using System.Text.Json;

using Ecomm.Customers.Api.Models;
using Ecomm.Customers.Api.Persistence;
using Ecomm.Customers.Api.Requests;
using Ecomm.Messaging;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Features.CreateCustomer;

public sealed class CreateCustomerHandler(
    IValidator<CreateCustomerRequest> validator,
    CustomersDbContext dbContext,
    TimeProvider timeProvider)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<Guid> ExecuteAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var document = Document.Create(request.Document);
        if (await dbContext.Customers.AnyAsync(customer => customer.Document == document, cancellationToken))
            throw new InvalidOperationException("This customer already exists.");

        var customer = Customer.Create(request.Name, request.Email, request.Document);
        var data = new CustomerCreatedIntegrationEvent(
            customer.Id, customer.Name, customer.Email.Value, customer.Document.Value);
        var now = timeProvider.GetUtcNow();
        var id = Guid.NewGuid();
        var correlationId = Activity.Current?.TraceId.ToString();
        var causationId = Activity.Current?.ParentSpanId.ToString();
        var envelope = new EventEnvelope<CustomerCreatedIntegrationEvent>(
            id, "customers.customer-created.v1", 1, now, correlationId, causationId, data);

        dbContext.Customers.Add(customer);
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = id,
            Type = envelope.Type,
            Version = envelope.Version,
            Destination = "customers-events",
            Payload = JsonSerializer.Serialize(envelope, SerializerOptions),
            OccurredAtUtc = now,
            NextAttemptAtUtc = now,
            CorrelationId = correlationId,
            CausationId = causationId,
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}

public sealed record CustomerCreatedIntegrationEvent(Guid Id, string Name, string Email, string Document);
