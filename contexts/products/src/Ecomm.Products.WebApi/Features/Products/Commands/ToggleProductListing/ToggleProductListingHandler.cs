using Ecomm.Products.WebApi.Features.Inventory.Application;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Products.Commands.ToggleProductListing;

public sealed class ToggleProductListingHandler(
    IProductRepository productRepository,
    IInventoryAvailabilityService inventoryAvailabilityService,
    IUnitOfWork unitOfWork)
{
    public async Task Handle(ToggleProductListingCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
            throw new NotFoundException($"Product {command.ProductId} not found.");

        if (command.List)
        {
            if (!await inventoryAvailabilityService.HasAvailableStockAsync(command.ProductId, cancellationToken))
                throw new DomainValidationException("Produto não pode ser listado sem estoque disponível.");
            if (!product.IsListed)
                product.ToggleListing();
        }
        else
        {
            if (product.IsListed)
                product.ToggleListing();
        }

        await productRepository.UpdateAsync(product, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
