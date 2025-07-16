using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Features.Products.Domain.Events;
using Ecomm.Shared.SeedWork;

using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Handlers;

public sealed class CreateItemOnInventoryDomainEventHandler(
    IInventoryRepository inventoryRepository
) : IDomainEventHandler<ProductCreatedDomainEvent>
{
    public async Task HandleAsync(ProductCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var inventoryItem = InventoryEntity.Create(domainEvent.ProductId, Quantity.Create(0));
        await inventoryRepository.AddAsync(inventoryItem, cancellationToken);
    }
}
