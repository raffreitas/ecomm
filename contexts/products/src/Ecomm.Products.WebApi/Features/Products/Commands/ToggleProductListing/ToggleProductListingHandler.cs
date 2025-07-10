using Ecomm.Products.WebApi.Features.Inventory.Application;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Products.Commands.ToggleProductListing;

public sealed class ToggleProductListingHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryAvailabilityService _inventoryAvailabilityService;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleProductListingHandler(
        IProductRepository productRepository,
        IInventoryAvailabilityService inventoryAvailabilityService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _inventoryAvailabilityService = inventoryAvailabilityService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ToggleProductListingCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
            throw new NotFoundException($"Product {command.ProductId} not found.");

        if (command.List)
        {
            // Só pode listar se houver estoque disponível
            if (!await _inventoryAvailabilityService.HasAvailableStockAsync(command.ProductId, cancellationToken))
                throw new DomainValidationException("Produto não pode ser listado sem estoque disponível.");
            if (!product.IsListed)
                product.ToggleListing();
        }
        else
        {
            if (product.IsListed)
                product.ToggleListing();
        }

        await _productRepository.AddAsync(product, cancellationToken); // ou UpdateAsync se existir
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
