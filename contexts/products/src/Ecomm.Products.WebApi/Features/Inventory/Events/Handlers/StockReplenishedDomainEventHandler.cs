using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Features.Inventory.Events.Integration;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockReplenishedDomainEventHandler(
    ILogger<StockReplenishedDomainEventHandler> logger,
    IEventOutboxService eventOutboxService
) : IDomainEventHandler<StockReplenishedDomainEvent>
{
    public async Task HandleAsync(StockReplenishedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Inventory] Stock replenished: Product {ProductId}, Current Quantity: {CurrentQuantity}", domainEvent.ProductId, domainEvent.CurrentQuantity);
        var integrationEvent = new StockReplenishedIntegrationEvent(domainEvent.AggregateId, domainEvent.ProductId, domainEvent.CurrentQuantity);
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        logger.LogInformation("[Inventory] Integration event StockReplenishedIntegrationEvent published for Product {ProductId}.", domainEvent.ProductId);
    }
}
