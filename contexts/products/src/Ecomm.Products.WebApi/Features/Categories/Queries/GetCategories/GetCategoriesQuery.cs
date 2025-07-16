namespace Ecomm.Products.WebApi.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery
{
    public Guid? ParentCategoryId { get; init; }
}
