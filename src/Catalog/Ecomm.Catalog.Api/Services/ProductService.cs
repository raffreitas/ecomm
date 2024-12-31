using Ecomm.Catalog.Api.Exceptions.Base;
using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Models.ViewModels;
using Ecomm.Catalog.Api.Repositories;
using Ecomm.Catalog.Api.Services.Contracts;

namespace Ecomm.Catalog.Api.Services;

public class ProductService(IProductRepository repository, ICategoryRepository categoryRepository) : IProductService
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
        var category =
            await categoryRepository.GetCategoryByIdAsync(createProductInputModel.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category not found");

        var product = new Product
        {
            Name = createProductInputModel.Name,
            Description = createProductInputModel.Description,
            Price = createProductInputModel.Price,
            ImageUrl = createProductInputModel.ImageUrl,
            Category = category,
        };

        await repository.CreateProductAsync(product, cancellationToken);
    }
}