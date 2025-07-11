namespace Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand
{
    public required string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }
}
