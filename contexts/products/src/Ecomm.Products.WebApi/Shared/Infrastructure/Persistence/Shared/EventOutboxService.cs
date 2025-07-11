using System.Text.Json;

using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Shared;

internal sealed class EventOutboxService(ApplicationDbContext dbContext) : IEventOutboxService
{
    public async Task AddAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        var outboxEvent = new OutboxEvent(
            typeof(T).FullName!,
            JsonSerializer.Serialize(integrationEvent),
            integrationEvent.OccurredAt
        );

        await dbContext.Set<OutboxEvent>().AddAsync(outboxEvent, cancellationToken);
    }
}
