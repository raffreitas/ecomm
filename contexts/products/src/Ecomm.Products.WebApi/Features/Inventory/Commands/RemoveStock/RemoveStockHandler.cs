using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;

internal sealed class RemoveStockHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<RemoveStockResult> Handle(RemoveStockCommand command, CancellationToken ct)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, ct);
        if (inventory is null)
            return RemoveStockResult.NotFound;

        var quantityToRemove = Quantity.Create(command.Quantity);
        
        if (!inventory.HasSufficientStock(quantityToRemove))
            return RemoveStockResult.InsufficientStock;

        inventory.RemoveStock(quantityToRemove);

        await unitOfWork.CommitAsync(ct);
        return RemoveStockResult.Success;
    }
}

public enum RemoveStockResult
{
    Success,
    NotFound,
    InsufficientStock
}
