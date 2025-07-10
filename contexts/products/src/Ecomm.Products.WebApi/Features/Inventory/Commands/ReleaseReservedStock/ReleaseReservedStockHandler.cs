using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;

public sealed class ReleaseReservedStockHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseReservedStockHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReleaseReservedStockCommand command, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory for product {command.ProductId} not found.");

        var quantityToRelease = Quantity.Create(command.Quantity);
        try
        {
            inventory.ReleaseReservedStock(quantityToRelease);
        }
        catch (ArgumentException ex)
        {
            throw new DomainValidationException(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainValidationException(ex.Message);
        }

        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
