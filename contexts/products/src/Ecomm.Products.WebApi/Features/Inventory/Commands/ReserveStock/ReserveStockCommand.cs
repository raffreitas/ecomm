namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReserveStock;

public sealed record ReserveStockCommand
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
