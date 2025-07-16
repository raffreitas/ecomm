using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Integration;

public sealed record StockReplenishedIntegrationEvent : IntegrationEvent
{
    public override int Version => 1;
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }
    public int CurrentQuantity { get; init; }

    public StockReplenishedIntegrationEvent(Guid inventoryId, Guid productId, int currentQuantity)
    {
        InventoryId = inventoryId;
        ProductId = productId;
        CurrentQuantity = currentQuantity;
    }
}
