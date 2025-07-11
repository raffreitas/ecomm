using Ecomm.Products.WebApi.Features.Categories.Domain;

namespace Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetByParentIdAsync(Guid? parentCategoryId, CancellationToken cancellationToken = default);
}
