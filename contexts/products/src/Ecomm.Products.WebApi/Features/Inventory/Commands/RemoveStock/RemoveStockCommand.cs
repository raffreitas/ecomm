using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;

public sealed record RemoveStockCommand
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}

public sealed class RemoveStockCommandValidator : AbstractValidator<RemoveStockCommand>
{
    public RemoveStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
