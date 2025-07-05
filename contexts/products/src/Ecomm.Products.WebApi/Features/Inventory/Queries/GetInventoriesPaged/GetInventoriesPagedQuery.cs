using FluentValidation;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoriesPaged;

public sealed record GetInventoriesPagedQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public sealed class GetInventoriesPagedQueryValidator : AbstractValidator<GetInventoriesPagedQuery>
{
    public GetInventoriesPagedQueryValidator()
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
