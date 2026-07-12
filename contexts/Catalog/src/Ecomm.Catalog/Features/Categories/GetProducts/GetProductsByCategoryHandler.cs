using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetProducts;

public sealed class GetProductsByCategoryHandler(CatalogDbContext dbContext)
{
    public async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == categoryId)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }

        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.CategoryId == categoryId)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                category))
            .ToListAsync(cancellationToken);
    }
}
