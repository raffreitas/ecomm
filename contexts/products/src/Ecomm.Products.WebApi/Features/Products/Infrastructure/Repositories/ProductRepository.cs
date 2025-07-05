using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Features.Products.Infrastructure.Repositories;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await dbContext.Products.AddAsync(product, cancellationToken);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Products
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

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
        var query = dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Description.Contains(searchTerm));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);
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

        return PagedResult<Product>.Create(items, totalCount, page, pageSize);
    }
}
