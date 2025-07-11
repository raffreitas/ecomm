using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Features.Inventory.Events.Integration;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockBelowMinimumDomainEventHandler(
    ILogger<StockBelowMinimumDomainEventHandler> logger,
    IEventOutboxService eventOutboxService
) : IDomainEventHandler<StockBelowMinimumDomainEvent>
{
    public async Task HandleAsync(StockBelowMinimumDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("[Inventory] Stock below minimum: Product {ProductId}, Current Quantity: {CurrentQuantity}, Minimum: {MinimumStockLevel}", domainEvent.ProductId, domainEvent.CurrentQuantity, domainEvent.MinimumStockLevel);

        // Trigger alert, open purchase ticket, etc.
        // Publish integration event for other contexts (RabbitMQ via Outbox)
        var integrationEvent = new StockBelowMinimumIntegrationEvent(
            domainEvent.AggregateId,
            domainEvent.ProductId,
            domainEvent.CurrentQuantity,
            domainEvent.MinimumStockLevel);
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        logger.LogInformation("[Inventory] Integration event StockBelowMinimumIntegrationEvent published for Product {ProductId}.", domainEvent.ProductId);
    }
}
