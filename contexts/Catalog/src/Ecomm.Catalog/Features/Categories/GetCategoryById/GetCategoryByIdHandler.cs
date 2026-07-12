using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetCategoryById;

public sealed class GetCategoryByIdHandler(CatalogDbContext dbContext)
{
    public async Task<CategoryResponse> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync(cancellationToken);

        return category ?? throw new NotFoundException("Category not found");
    }
}
