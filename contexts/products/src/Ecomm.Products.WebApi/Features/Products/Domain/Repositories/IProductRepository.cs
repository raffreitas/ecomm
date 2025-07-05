using Ecomm.Products.WebApi.Features.Products.Domain;

namespace Ecomm.Products.WebApi.Features.Products.Domain.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
}
