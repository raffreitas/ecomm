using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Features.Inventory.Events.Integration;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class StockCreatedDomainEventHandler(
    ILogger<StockCreatedDomainEventHandler> logger,
    IEventOutboxService eventOutboxService
) : IDomainEventHandler<StockCreatedDomainEvent>
{
    public async Task HandleAsync(StockCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Inventory] New stock created: Product {ProductId}, Initial Quantity: {InitialQuantity}", domainEvent.ProductId, domainEvent.InitialQuantity);
        var integrationEvent = new StockCreatedIntegrationEvent(domainEvent.AggregateId, domainEvent.ProductId, domainEvent.InitialQuantity);
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        logger.LogInformation("[Inventory] Integration event StockCreatedIntegrationEvent published for Product {ProductId}.", domainEvent.ProductId);
    }
}
