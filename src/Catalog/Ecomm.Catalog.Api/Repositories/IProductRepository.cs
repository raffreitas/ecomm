using Ecomm.Catalog.Models;

namespace Ecomm.Catalog.Repositories;

public interface IProductRepository
{
    Task<IList<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IList<Product>> GetProductByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task CreateProductAsync(Product product, CancellationToken cancellationToken = default);
}