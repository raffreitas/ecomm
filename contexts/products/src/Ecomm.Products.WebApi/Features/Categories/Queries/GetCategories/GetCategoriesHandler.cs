using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;

namespace Ecomm.Products.WebApi.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetByParentIdAsync(query.ParentCategoryId, cancellationToken);
        return [.. categories.Select(CategoryDto.FromEntity)];
    }
}

public sealed record CategoryDto(Guid Id, string Name, Guid? ParentCategoryId)
{
    public static CategoryDto FromEntity(Category category) =>
        new(category.Id, category.Name, category.ParentCategoryId);
}
