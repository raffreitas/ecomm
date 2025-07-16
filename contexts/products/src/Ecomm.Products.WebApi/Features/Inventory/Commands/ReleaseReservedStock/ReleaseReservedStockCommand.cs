namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;

public sealed record ReleaseReservedStockCommand
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
