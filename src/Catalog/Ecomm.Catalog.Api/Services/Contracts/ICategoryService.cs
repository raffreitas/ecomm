using Ecomm.Catalog.Models.InputModel;
using Ecomm.Catalog.Models.ViewModels;

namespace Ecomm.Catalog.Services.Contracts;

public interface ICategoryService
{
    Task<IList<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryViewModel?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task CreateCategoryAsync(CreateCategoryInputModel category, CancellationToken cancellationToken = default);
}