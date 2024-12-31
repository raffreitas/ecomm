using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Api.Persistence.Repositories;

public class ProductRepository(CatalogDbContext dbContext) : IProductRepository
{
    public async Task<IList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Products.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);


    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<IList<Product>> GetProductByCategoryIdAsync(Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(p => p.CategoryId.Equals(categoryId))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}