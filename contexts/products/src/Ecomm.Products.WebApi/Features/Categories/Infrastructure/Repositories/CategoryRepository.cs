using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Features.Categories.Infrastructure.Repositories;

public sealed class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => await dbContext.Categories.AddAsync(category, cancellationToken);

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        dbContext.Categories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        dbContext.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetByParentIdAsync(Guid? parentCategoryId, CancellationToken cancellationToken = default)
        => await dbContext.Categories
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .ToListAsync(cancellationToken);
}
