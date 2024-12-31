using Ecomm.Orders.Domain.Entities;

namespace Ecomm.Orders.Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}