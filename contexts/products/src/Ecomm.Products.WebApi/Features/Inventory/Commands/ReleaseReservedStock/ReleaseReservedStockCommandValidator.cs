using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;

public class ReleaseReservedStockCommandValidator : AbstractValidator<ReleaseReservedStockCommand>
{
    public ReleaseReservedStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
