using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Models.ViewModels;

namespace Ecomm.Catalog.Api.Services.Contracts;

public interface ICategoryService
{
    Task<IList<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryViewModel?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task CreateCategoryAsync(CreateCategoryInputModel category, CancellationToken cancellationToken = default);
}