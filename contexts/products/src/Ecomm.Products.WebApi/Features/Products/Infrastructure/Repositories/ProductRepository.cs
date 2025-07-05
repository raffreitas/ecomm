using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

namespace Ecomm.Products.WebApi.Features.Products.Infrastructure.Repositories;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await dbContext.Products.AddAsync(product, cancellationToken);
}
