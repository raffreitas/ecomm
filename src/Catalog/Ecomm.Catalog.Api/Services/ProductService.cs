using System.Text;
using System.Text.Json;

using Ecomm.Catalog.Api.Exceptions.Base;
using Ecomm.Catalog.Api.Messaging;
using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Models.ViewModels;
using Ecomm.Catalog.Api.Repositories;
using Ecomm.Catalog.Api.Services.Contracts;

namespace Ecomm.Catalog.Api.Services;

public class ProductService(
    IProductRepository repository,
    ICategoryRepository categoryRepository,
    IMessageBusService messageBusService) : IProductService
{
    public async Task<IList<ProductViewModel>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetProductsAsync(cancellationToken);

        return products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
        }).ToList();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetProductByIdAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException("Product not found");

        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product?.Category?.Name
        };
    }

    public async Task<IList<ProductViewModel>> GetProductByCategoryIdAsync(Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetProductByCategoryIdAsync(categoryId, cancellationToken);

        return products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name
        }).ToList();
    }

    public async Task CreateProductAsync(CreateProductInputModel createProductInputModel,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.ExistsByIdAsync(createProductInputModel.CategoryId, cancellationToken);
        if (!category)
            throw new NotFoundException("Category not found");

        var product = new Product
        {
            Name = createProductInputModel.Name,
            Description = createProductInputModel.Description,
            Price = createProductInputModel.Price,
            ImageUrl = createProductInputModel.ImageUrl,
            CategoryId = createProductInputModel.CategoryId,
        };

        await repository.CreateProductAsync(product, cancellationToken);

        var productCreatedMessage = JsonSerializer.Serialize(product);
        await messageBusService.PublishAsync(
            queue: "product.created",
            message: Encoding.UTF8.GetBytes(productCreatedMessage),
            cancellationToken);
    }
}