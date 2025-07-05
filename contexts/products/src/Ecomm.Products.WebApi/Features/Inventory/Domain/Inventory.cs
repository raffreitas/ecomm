using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain;

public sealed class Inventory : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public Quantity Quantity { get; private set; }
    public bool IsAvailable => Quantity.Value > 0;


    #region EF Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Inventory()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    #endregion

    private Inventory(Guid productId, Quantity quantity)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        ProductId = productId;
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity), "Quantity cannot be null.");
    }

    public static Inventory Create(Guid productId, Quantity quantity)
    {
        return new Inventory(productId, quantity);
    }
}
