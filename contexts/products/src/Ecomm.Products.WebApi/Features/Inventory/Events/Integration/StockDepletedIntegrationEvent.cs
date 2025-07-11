using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Integration;

public sealed record StockDepletedIntegrationEvent : IntegrationEvent
{
    public override int Version => 1;
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }

    public StockDepletedIntegrationEvent(Guid inventoryId, Guid productId)
    {
        InventoryId = inventoryId;
        ProductId = productId;
    }
}
