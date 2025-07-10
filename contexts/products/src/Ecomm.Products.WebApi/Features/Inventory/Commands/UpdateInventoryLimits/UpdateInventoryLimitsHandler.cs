using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;

public sealed class UpdateInventoryLimitsHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryLimitsHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateInventoryLimitsCommand command, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory for product {command.ProductId} not found.");

        var min = Quantity.Create(command.MinimumStockLevel);
        var max = Quantity.Create(command.MaximumStockLevel);

        inventory.UpdateMinimumStockLevel(min);
        inventory.UpdateMaximumStockLevel(max);

        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
