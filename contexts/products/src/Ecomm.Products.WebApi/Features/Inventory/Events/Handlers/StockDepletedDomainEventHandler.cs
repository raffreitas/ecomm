using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Features.Inventory.Events.Integration;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockDepletedDomainEventHandler(
    ILogger<StockDepletedDomainEventHandler> logger,
    IEventOutboxService eventOutboxService,
    IProductRepository productRepository
) : IDomainEventHandler<StockDepletedDomainEvent>
{
    public async Task HandleAsync(StockDepletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogError("[Inventory] Stock depleted: Product {ProductId}.", domainEvent.ProductId);

        var product = await productRepository.GetByIdAsync(domainEvent.ProductId, cancellationToken);
        if (product is not null && product.IsListed)
        {
            product.ToggleListing();
            await productRepository.UpdateAsync(product, cancellationToken);
            logger.LogInformation("[Inventory] Product {ProductId} sale blocked.", domainEvent.ProductId);
        }

        var integrationEvent = new StockDepletedIntegrationEvent(domainEvent.AggregateId, domainEvent.ProductId);
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        logger.LogInformation("[Inventory] Integration event StockDepletedIntegrationEvent published for Product {ProductId}.", domainEvent.ProductId);
    }
}
