using FluentValidation;
using FluentValidation.Results;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery
{
    public required Guid Id { get; init; }

    public ValidationResult Validate() => new GetProductByIdQueryValidator().Validate(this);
}

public sealed class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
