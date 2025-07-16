using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;

public sealed class UpdateInventoryLimitsHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
{
    public async Task Handle(UpdateInventoryLimitsCommand command, CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory for product {command.ProductId} not found.");

        var min = Quantity.Create(command.MinimumStockLevel);
        var max = Quantity.Create(command.MaximumStockLevel);

        inventory.UpdateMinimumStockLevel(min);
        inventory.UpdateMaximumStockLevel(max);

        await inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
