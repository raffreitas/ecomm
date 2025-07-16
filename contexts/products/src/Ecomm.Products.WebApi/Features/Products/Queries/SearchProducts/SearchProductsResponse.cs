namespace Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

public sealed record SearchProductsResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
    public required bool IsListed { get; init; }
    public required string[] Categories { get; init; } = [];
    public required DateTimeOffset CreatedAt { get; init; }
}
