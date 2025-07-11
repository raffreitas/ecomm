namespace Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;

public sealed record UpdateInventoryLimitsCommand
{
    public required Guid ProductId { get; init; }
    public required int MinimumStockLevel { get; init; }
    public required int MaximumStockLevel { get; init; }
}
