using System.Text.Json;

using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Infrastructure.Persistence;
using Ecomm.Catalog.Infrastructure.Persistence.Entities;

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
        var envelope = new EventEnvelope<TEvent>(
            Guid.NewGuid(),
            metadata.Type,
            metadata.Version,
            timeProvider.GetUtcNow(),
            integrationEvent);

        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = envelope.Id,
            Type = metadata.Type,
            Version = metadata.Version,
            Destination = metadata.Destination,
            Payload = JsonSerializer.Serialize(envelope, SerializerOptions),
            OccurredAtUtc = envelope.OccurredAtUtc,
            NextAttemptAtUtc = envelope.OccurredAtUtc,
        });

        return Task.CompletedTask;
    }
}
