using Ecomm.Products.WebApi.Features.Inventory.Domain.Events;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain;

public sealed class Inventory : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public Quantity Quantity { get; private set; }
    public Quantity ReservedQuantity { get; private set; }
    public Quantity MinimumStockLevel { get; private set; }
    public Quantity MaximumStockLevel { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public bool IsAvailable => Quantity > 0 && IsActive;

    #region EF Constructor
#pragma warning disable CS8618
    private Inventory() { }
#pragma warning restore CS8618
    #endregion

    private Inventory(Guid productId, Quantity quantity, Quantity minimumStockLevel, Quantity maximumStockLevel)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        ProductId = productId;
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity), "Quantity cannot be null.");
        ReservedQuantity = Quantity.Create(0);
        MinimumStockLevel = minimumStockLevel ?? Quantity.Create(0);
        MaximumStockLevel = maximumStockLevel ?? Quantity.Create(int.MaxValue);
        IsActive = true;
        LastUpdated = DateTime.UtcNow;

        AddDomainEvent(new StockCreatedDomainEvent(
            Id,
            ProductId,
            Quantity,
            MinimumStockLevel,
            MaximumStockLevel
        ));
    }

    public static Inventory Create(Guid productId, Quantity quantity, Quantity? minimumStockLevel = null,
        Quantity? maximumStockLevel = null)
    {
        return new Inventory(productId, quantity, minimumStockLevel ?? Quantity.Create(0),
            maximumStockLevel ?? Quantity.Create(int.MaxValue));
    }

    public void AddStock(Quantity quantityToAdd)
    {
        if (quantityToAdd is null)
            throw new ArgumentNullException(nameof(quantityToAdd), "Quantity to add cannot be null.");
        if (quantityToAdd.Value <= 0)
            throw new ArgumentException("Quantity to add must be positive.", nameof(quantityToAdd));
        Quantity = Quantity.Create(Quantity + quantityToAdd);
        LastUpdated = DateTime.UtcNow;
        if (IsBelowMinimum())
        {
            AddDomainEvent(new StockReplenishedDomainEvent(Id, ProductId, Quantity.Value));
        }

        if (IsBelowMinimum())
        {
            AddDomainEvent(new StockReplenishedDomainEvent(Id, ProductId, Quantity.Value));
        }
    }

    public void RemoveStock(Quantity quantityToRemove)
    {
        if (quantityToRemove is null)
            throw new ArgumentNullException(nameof(quantityToRemove), "Quantity to remove cannot be null.");
        if (quantityToRemove <= 0)
            throw new ArgumentException("Quantity to remove must be positive.", nameof(quantityToRemove));
        if (Quantity < quantityToRemove)
            throw new InvalidOperationException("Cannot remove more stock than available.");
        Quantity = Quantity.Create(Quantity - quantityToRemove);
        LastUpdated = DateTime.UtcNow;
        if (Quantity == 0)
        {
            AddDomainEvent(new StockDepletedDomainEvent(Id, ProductId));
        }
        else if (IsBelowMinimum())
        {
            AddDomainEvent(new StockBelowMinimumDomainEvent(Id, ProductId, Quantity, MinimumStockLevel));
        }

        if (Quantity == 0)
        {
            AddDomainEvent(new StockDepletedDomainEvent(Id, ProductId));
        }
        else if (IsBelowMinimum())
        {
            AddDomainEvent(new StockBelowMinimumDomainEvent(Id, ProductId, Quantity, MinimumStockLevel));
        }
    }

    public void ReserveStock(Quantity quantityToReserve)
    {
        if (quantityToReserve is null)
            throw new ArgumentNullException(nameof(quantityToReserve), "Quantity to reserve cannot be null.");
        if (quantityToReserve <= 0)
            throw new ArgumentException("Quantity to reserve must be positive.", nameof(quantityToReserve));
        if (AvailableStock() < quantityToReserve)
            throw new InvalidOperationException("Not enough available stock to reserve.");
        ReservedQuantity = Quantity.Create(ReservedQuantity + quantityToReserve);
        LastUpdated = DateTime.UtcNow;
    }

    public void ReleaseReservedStock(Quantity quantityToRelease)
    {
        if (quantityToRelease is null)
            throw new ArgumentNullException(nameof(quantityToRelease), "Quantity to release cannot be null.");
        if (quantityToRelease <= 0)
            throw new ArgumentException("Quantity to release must be positive.", nameof(quantityToRelease));
        if (ReservedQuantity < quantityToRelease)
            throw new InvalidOperationException("Cannot release more than reserved.");
        ReservedQuantity = Quantity.Create(ReservedQuantity - quantityToRelease);
        LastUpdated = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastUpdated = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        LastUpdated = DateTime.UtcNow;
    }

    public bool IsBelowMinimum()
        => Quantity < MinimumStockLevel;

    public bool IsAboveMaximum()
        => Quantity > MaximumStockLevel;

    public void UpdateMinimumStockLevel(Quantity newMin)
    {
        if (newMin is null)
            throw new ArgumentNullException(nameof(newMin), "Minimum stock level cannot be null.");
        MinimumStockLevel = newMin;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateMaximumStockLevel(Quantity newMax)
    {
        if (newMax is null)
            throw new ArgumentNullException(nameof(newMax), "Maximum stock level cannot be null.");
        MaximumStockLevel = newMax;
        LastUpdated = DateTime.UtcNow;
    }

    public Quantity AvailableStock()
        => Quantity.Create(Quantity - ReservedQuantity);


    public bool HasSufficientStock(Quantity requiredQuantity)
    {
        if (requiredQuantity is null)
            throw new ArgumentNullException(nameof(requiredQuantity), "Required quantity cannot be null.");
        return AvailableStock() >= requiredQuantity;
    }
}