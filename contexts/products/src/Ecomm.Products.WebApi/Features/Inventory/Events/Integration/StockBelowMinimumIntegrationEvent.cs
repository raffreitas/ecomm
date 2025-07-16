using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Integration;

public sealed record StockBelowMinimumIntegrationEvent : IntegrationEvent
{
    public override int Version => 1;
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }
    public int CurrentQuantity { get; init; }
    public int MinimumStockLevel { get; init; }

    public StockBelowMinimumIntegrationEvent(Guid inventoryId, Guid productId, int currentQuantity, int minimumStockLevel)
    {
        InventoryId = inventoryId;
        ProductId = productId;
        CurrentQuantity = currentQuantity;
        MinimumStockLevel = minimumStockLevel;
    }
}
