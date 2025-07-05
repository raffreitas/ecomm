using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.AddStock;

internal sealed class AddStockHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<bool> Handle(AddStockCommand command, CancellationToken ct)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, ct);
        if (inventory is null)
            return false;

        var quantityToAdd = Quantity.Create(command.Quantity);
        inventory.AddStock(quantityToAdd);

        await unitOfWork.CommitAsync(ct);
        return true;
    }
}
