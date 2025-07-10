using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Events;

public sealed record StockCreatedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public Guid InventoryId { get; }
    public Guid ProductId { get; }
    public int InitialQuantity { get; }
    public int MinimumStockLevel { get; }
    public int MaximumStockLevel { get; }

    public StockCreatedDomainEvent(Guid inventoryId, Guid productId, int initialQuantity, int minimumStockLevel, int maximumStockLevel)
    {
        AggregateId = inventoryId;
        InventoryId = inventoryId;
        ProductId = productId;
        InitialQuantity = initialQuantity;
        MinimumStockLevel = minimumStockLevel;
        MaximumStockLevel = maximumStockLevel;
    }
}
