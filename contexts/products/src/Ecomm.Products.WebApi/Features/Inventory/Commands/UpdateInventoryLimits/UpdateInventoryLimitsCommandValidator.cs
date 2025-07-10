using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;

public class UpdateInventoryLimitsCommandValidator : AbstractValidator<UpdateInventoryLimitsCommand>
{
    public UpdateInventoryLimitsCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("MinimumStockLevel must be zero or greater.");

        RuleFor(x => x.MaximumStockLevel)
            .GreaterThan(0).WithMessage("MaximumStockLevel must be greater than zero.");

        RuleFor(x => x)
            .Must(x => x.MaximumStockLevel > x.MinimumStockLevel)
            .WithMessage("MaximumStockLevel must be greater than MinimumStockLevel.");
    }
}
