using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoryByProductId;

public sealed record GetInventoryByProductIdQuery
{
    public required Guid ProductId { get; init; }
}

public sealed class GetInventoryByProductIdQueryValidator : AbstractValidator<GetInventoryByProductIdQuery>
{
    public GetInventoryByProductIdQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
