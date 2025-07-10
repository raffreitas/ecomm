namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReserveStock;

public sealed class ReserveStockCommand
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
