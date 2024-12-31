using Ecomm.Catalog.Api.Models;

namespace Ecomm.Catalog.Api.Repositories;

public interface ICategoryRepository
{
    Task<IList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateCategoryAsync(Category category, CancellationToken cancellationToken = default);
}