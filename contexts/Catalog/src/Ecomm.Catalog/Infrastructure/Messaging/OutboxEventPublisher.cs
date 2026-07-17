using System.Text.Json;
using System.Diagnostics;

using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Infrastructure.Persistence;
using Ecomm.Catalog.Infrastructure.Persistence.Entities;
using Ecomm.Messaging;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class OutboxEventPublisher(
    CatalogDbContext dbContext,
    IEventResolver eventResolver,
    TimeProvider timeProvider) : IEventPublisher
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task PublishAsync<TEvent>(
        TEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        cancellationToken.ThrowIfCancellationRequested();

        var metadata = eventResolver.Resolve<TEvent>();
        var correlationId = Activity.Current?.TraceId.ToString();
        var causationId = Activity.Current?.ParentSpanId.ToString();
        var envelope = new EventEnvelope<TEvent>(
            Guid.NewGuid(),
            metadata.Type,
            metadata.Version,
            timeProvider.GetUtcNow(),
            correlationId,
            causationId,
            integrationEvent);

        dbContext.OutboxMessages.Add(new Ecomm.Catalog.Infrastructure.Persistence.Entities.OutboxMessage
        {
            Id = envelope.Id,
            Type = metadata.Type,
            Version = metadata.Version,
            Destination = metadata.Destination,
            Payload = JsonSerializer.Serialize(envelope, SerializerOptions),
            CorrelationId = correlationId,
            CausationId = causationId,
            OccurredAtUtc = envelope.OccurredAtUtc,
            NextAttemptAtUtc = envelope.OccurredAtUtc,
        });

        return Task.CompletedTask;
    }
}
