using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReserveStock;

public sealed class ReserveStockHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReserveStockCommand command, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory for product {command.ProductId} not found.");

        var quantityToReserve = Quantity.Create(command.Quantity);
        try
        {
            inventory.ReserveStock(quantityToReserve);
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
