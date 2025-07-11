namespace Ecomm.Products.WebApi.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }
}
