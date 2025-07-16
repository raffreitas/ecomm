using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events;

public sealed record StockBelowMinimumDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public Guid InventoryId { get; }
    public Guid ProductId { get; }
    public int CurrentQuantity { get; }
    public int MinimumStockLevel { get; }


    public StockBelowMinimumDomainEvent(Guid inventoryId, Guid productId, int currentQuantity, int minimumStockLevel)
    {
        AggregateId = inventoryId;
        InventoryId = inventoryId;
        ProductId = productId;
        CurrentQuantity = currentQuantity;
        MinimumStockLevel = minimumStockLevel;
    }
}
