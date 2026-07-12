using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.GetProducts;

public sealed class GetProductsHandler(CatalogDbContext dbContext)
{
    public async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                new CategoryResponse(product.CategoryId, product.Category!.Name)))
            .ToListAsync(cancellationToken);
    }
}
