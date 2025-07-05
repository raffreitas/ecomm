using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.CreateInventory;

internal sealed class CreateInventoryHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> Handle(CreateInventoryCommand command, CancellationToken ct)
    {
        // Verificar se já existe inventário para o produto
        var existingInventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, ct);
        if (existingInventory is not null)
            throw new InvalidOperationException($"Inventory already exists for product {command.ProductId}");

        var quantity = Quantity.Create(command.Quantity);
        var inventory = InventoryEntity.Create(command.ProductId, quantity);

        await inventoryRepository.AddAsync(inventory, ct);
        await unitOfWork.CommitAsync(ct);

        return inventory.Id;
    }
}
