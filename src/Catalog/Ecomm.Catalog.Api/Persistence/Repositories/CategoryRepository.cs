using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Api.Persistence.Repositories;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public async Task<IList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.Equals(id), cancellationToken);
    }

    public async Task CreateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories.AsNoTracking().AnyAsync(c => c.Id.Equals(id), cancellationToken);
    }
}