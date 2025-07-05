using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.CreateInventory;

public sealed record CreateInventoryCommand
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}

public sealed class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater than or equal to 0.");
    }
}
