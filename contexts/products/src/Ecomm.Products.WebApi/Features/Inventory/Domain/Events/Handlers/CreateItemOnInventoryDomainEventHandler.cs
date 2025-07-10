using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Features.Products.Domain.Events;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events.Handlers;

public sealed class CreateItemOnInventoryDomainEventHandler(
    IInventoryRepository inventoryRepository
) : IDomainEventHandler<ProductCreatedDomainEvent>
{
    public async Task HandleAsync(ProductCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var inventoryItem = Inventory.Create(domainEvent.ProductId, Quantity.Create(0));
        await inventoryRepository.AddAsync(inventoryItem, cancellationToken);
    }
}
