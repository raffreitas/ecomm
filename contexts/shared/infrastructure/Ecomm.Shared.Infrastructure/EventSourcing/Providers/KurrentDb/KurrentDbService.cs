using System.Text.Json;

using Ecomm.Shared.Infrastructure.EventSourcing.Abstractions;
using Ecomm.Shared.SeedWork;

using KurrentDB.Client;

namespace Ecomm.Shared.Infrastructure.EventSourcing.Providers.KurrentDb;

internal sealed class KurrentDbService(KurrentDBClient client) : IEventStoreService
{
    public async Task SaveEvent<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : DomainEvent
    {
        await client.AppendToStreamAsync(
            @event.AggregateId.ToString(),
            StreamState.Any,
            FormatEvent(@event),
            cancellationToken: ct);
    }

    private static IEnumerable<EventData> FormatEvent<TEvent>(TEvent @event) where TEvent : DomainEvent
    {
        yield return new EventData(
            Uuid.NewUuid(),
            @event.Type,
            JsonSerializer.SerializeToUtf8Bytes(@event));
    }
}
