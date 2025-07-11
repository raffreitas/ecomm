using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;

public sealed class ReleaseReservedStockHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
{
    public async Task Handle(ReleaseReservedStockCommand command, CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
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

        await inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
