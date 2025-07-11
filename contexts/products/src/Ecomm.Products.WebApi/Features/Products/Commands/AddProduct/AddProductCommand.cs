using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;

public sealed record AddProductCommand
{
    public required Guid[] CategoryIds { get; init; } = [];
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
}

public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Product description is required.")
            .MaximumLength(500)
            .WithMessage("Product description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be exactly 3 characters.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category is required.");
    }
}