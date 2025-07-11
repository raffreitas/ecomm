namespace Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommand
{
    public required string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }
}
