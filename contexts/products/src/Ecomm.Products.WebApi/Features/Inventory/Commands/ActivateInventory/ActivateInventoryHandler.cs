using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;

public sealed class ActivateInventoryHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
{
    public async Task Handle(ActivateInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory for product {command.ProductId} not found.");

        inventory.Activate();
        await inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
