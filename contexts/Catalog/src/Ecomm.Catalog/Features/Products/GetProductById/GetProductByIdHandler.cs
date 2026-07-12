using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(CatalogDbContext dbContext)
{
    public async Task<ProductResponse> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                new CategoryResponse(product.CategoryId, product.Category!.Name)))
            .SingleOrDefaultAsync(cancellationToken);

        return product ?? throw new NotFoundException("Product not found");
    }
}
