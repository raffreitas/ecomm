using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Events.Integration;

public sealed record StockCreatedIntegrationEvent : IntegrationEvent
{
    public override int Version => 1;
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }
    public int InitialQuantity { get; init; }

    public StockCreatedIntegrationEvent(Guid inventoryId, Guid productId, int initialQuantity)
    {
        InventoryId = inventoryId;
        ProductId = productId;
        InitialQuantity = initialQuantity;
    }
}
