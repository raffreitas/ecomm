using FluentValidation;
using FluentValidation.Results;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductsPaged;

public sealed record GetProductsPagedQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public ValidationResult Validate() => new GetProductsPagedQueryValidator().Validate(this);
}

public sealed class GetProductsPagedQueryValidator : AbstractValidator<GetProductsPagedQuery>
{
    public GetProductsPagedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
