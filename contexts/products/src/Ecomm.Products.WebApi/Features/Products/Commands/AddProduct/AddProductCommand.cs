using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;

public sealed record AddProductCommand
{
    public required string[] Categories { get; init; } = [];
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
}


public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
    }
}