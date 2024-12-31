using Ecomm.Catalog.Api.Exceptions.Base;
using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Models.ViewModels;
using Ecomm.Catalog.Api.Repositories;
using Ecomm.Catalog.Api.Services.Contracts;

namespace Ecomm.Catalog.Api.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<IList<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetCategoriesAsync(cancellationToken);

        return categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, }).ToList();
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryByIdAsync(categoryId, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found ");

        return new CategoryViewModel { Id = category.Id, Name = category.Name, };
    }

    public async Task CreateCategoryAsync(CreateCategoryInputModel categoryInputModel,
        CancellationToken cancellationToken = default)
    {
        var category = new Category { Name = categoryInputModel.Name };
        await repository.CreateCategoryAsync(category, cancellationToken);
    }
}