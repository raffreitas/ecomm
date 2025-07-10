using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Features.Inventory.Exceptions;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;

internal sealed class RemoveStockHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task Handle(RemoveStockCommand command, CancellationToken ct)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, ct);
        if (inventory is null)
            throw new NotFoundException($"Inventory for Product with ID '{command.ProductId}' not found.");

        var quantityToRemove = Quantity.Create(command.Quantity);

        if (!inventory.HasSufficientStock(quantityToRemove))
            throw new NoSufficientStockException(command.ProductId, quantityToRemove.Value);

        inventory.RemoveStock(quantityToRemove);
        await inventoryRepository.UpdateAsync(inventory, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
