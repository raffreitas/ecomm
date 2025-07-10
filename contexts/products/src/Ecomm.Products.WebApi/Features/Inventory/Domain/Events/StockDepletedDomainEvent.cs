using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events;

public sealed record StockDepletedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public Guid InventoryId { get; }
    public Guid ProductId { get; }

    public StockDepletedDomainEvent(Guid inventoryId, Guid productId)
    {
        AggregateId = inventoryId;
        InventoryId = inventoryId;
        ProductId = productId;
    }
}
