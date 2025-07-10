using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.AddStock;

internal sealed class AddStockHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<bool> Handle(AddStockCommand command, CancellationToken ct)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, ct);
        if (inventory is null)
            throw new NotFoundException($"Inventory for Product with ID '{command.ProductId}' not found.");

        var quantityToAdd = Quantity.Create(command.Quantity);
        inventory.AddStock(quantityToAdd);

        await inventoryRepository.UpdateAsync(inventory, ct);
        await unitOfWork.CommitAsync(ct);
        return true;
    }
}
