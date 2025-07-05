using FluentValidation;
using FluentValidation.Results;

namespace Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

public sealed record SearchProductsQuery
{
    public string? SearchTerm { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Currency { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public ValidationResult Validate() => new SearchProductsQueryValidator().Validate(this);
}

public sealed class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be greater than or equal to 0.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to 0.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to minimum price.");

        RuleFor(x => x.Currency)
            .Length(3)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Currency must be exactly 3 characters.");

        RuleFor(x => x)
            .Must(HaveAtLeastOneSearchCriteria)
            .WithMessage("At least one search criteria must be provided (SearchTerm, MinPrice, MaxPrice, or Currency).");
    }

    private static bool HaveAtLeastOneSearchCriteria(SearchProductsQuery query)
    {
        return !string.IsNullOrWhiteSpace(query.SearchTerm) ||
               query.MinPrice.HasValue ||
               query.MaxPrice.HasValue ||
               !string.IsNullOrWhiteSpace(query.Currency);
    }
}
