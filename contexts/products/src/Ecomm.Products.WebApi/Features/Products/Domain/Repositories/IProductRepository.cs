using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Products.Domain.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> SearchAsync(
        string? searchTerm, 
        decimal? minPrice, 
        decimal? maxPrice, 
        string? currency, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}
