namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
    public required bool IsListed { get; init; }
    public required string[] Categories { get; init; } = [];
    public required ImageResponse[] Images { get; init; } = [];
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed record ImageResponse
{
    public required string Url { get; init; }
    public required string AltText { get; init; }
}
