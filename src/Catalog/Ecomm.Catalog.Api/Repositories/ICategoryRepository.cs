using Ecomm.Catalog.Models;

namespace Ecomm.Catalog.Repositories;

public interface ICategoryRepository
{
    Task<IList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}