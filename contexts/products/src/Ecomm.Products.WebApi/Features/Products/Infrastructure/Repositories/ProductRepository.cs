using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Features.Products.Infrastructure.Repositories;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);

        var productCategories = product.CategoryIds.Select(categoryId => new ProductCategory(product.Id, categoryId));
        if (productCategories.Any())
            await dbContext.ProductsCategories.AddRangeAsync(productCategories, cancellationToken);
    }


    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return null;

        var categoryIds = await dbContext.ProductsCategories
            .Where(pc => pc.ProductId == id)
            .Select(pc => pc.CategoryId)
            .ToListAsync(cancellationToken);

        categoryIds.ForEach(product.AddCategory);

        return product;
    }


    public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Carregar os CategoryIds para cada produto
        foreach (var product in items)
        {
            var categoryIds = await dbContext.ProductsCategories
                .Where(pc => pc.ProductId == product.Id)
                .Select(pc => pc.CategoryId)
                .ToListAsync(cancellationToken);
            var field = typeof(Product).GetField("_categoryIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(product, categoryIds);
        }

        return PagedResult<Product>.Create(items, totalCount, page, pageSize);
    }


    public async Task<PagedResult<Product>> SearchAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        string? currency,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var connection = dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var query = dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Description.Contains(searchTerm));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount >= minPrice);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount <= maxPrice);
        }

        if (!string.IsNullOrWhiteSpace(currency))
        {
            query = query.Where(p => p.Price.Currency == currency);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.OrderBy(p => p.Name.Contains(searchTerm) ? 0 : 1)
                         .ThenBy(p => p.Name)
                         .ThenBy(p => p.CreatedAt);
        }
        else
        {
            query = query.OrderBy(p => p.Name);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        foreach (var product in items)
        {
            var categoryIds = await dbContext.ProductsCategories
                .Where(pc => pc.ProductId == product.Id)
                .Select(pc => pc.CategoryId)
                .ToListAsync(cancellationToken);
            categoryIds.ForEach(product.AddCategory);
        }

        return PagedResult<Product>.Create(items, totalCount, page, pageSize);
    }
}
