using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events;

public sealed record StockReplenishedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public Guid InventoryId { get; }
    public Guid ProductId { get; }
    public int CurrentQuantity { get; }

    public StockReplenishedDomainEvent(Guid inventoryId, Guid productId,  int currentQuantity)
    {
        AggregateId = inventoryId;
        InventoryId = inventoryId;
        ProductId = productId;
        CurrentQuantity = currentQuantity;
    }
}
