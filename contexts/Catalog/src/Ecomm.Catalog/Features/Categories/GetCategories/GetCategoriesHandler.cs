using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetCategories;

public sealed class GetCategoriesHandler(CatalogDbContext dbContext)
{
    public async Task<IReadOnlyList<CategoryResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .ToListAsync(cancellationToken);
    }
}
