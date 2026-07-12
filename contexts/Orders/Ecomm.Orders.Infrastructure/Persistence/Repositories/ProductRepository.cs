using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrdersDbContext _dbContext;

    public ProductRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}