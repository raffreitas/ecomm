namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoriesPaged;

public sealed record InventorySummaryResponse
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
    public required bool IsAvailable { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
