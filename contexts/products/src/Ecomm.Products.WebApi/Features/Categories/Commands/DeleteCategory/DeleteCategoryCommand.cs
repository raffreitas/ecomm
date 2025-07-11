namespace Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand
{
    public required Guid Id { get; init; }
}
