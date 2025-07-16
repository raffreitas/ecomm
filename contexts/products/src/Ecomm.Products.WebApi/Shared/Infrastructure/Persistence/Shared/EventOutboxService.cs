using System.Text.Json;

using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;
using Ecomm.Shared.Infrastructure.Observability.Correlation.Context;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Shared;

internal sealed class EventOutboxService(
    ApplicationDbContext dbContext,
    ICorrelationContextAccessor correlationContextAccessor
) : IEventOutboxService
{
    public async Task AddAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        var correlationId = correlationContextAccessor.Context?.CorrelationId ?? Guid.NewGuid().ToString("N");
        var outboxEvent = new OutboxEvent(
            typeof(T).FullName!,
            JsonSerializer.Serialize(integrationEvent),
            integrationEvent.OccurredAt,
            correlationId
        );

        await dbContext.Set<OutboxEvent>().AddAsync(outboxEvent, cancellationToken);
    }
}
