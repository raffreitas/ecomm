using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class OrderItem : Entity
{
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid OrderId { get; private set; }

    // EF. Rel
    public Product Product { get; private set; } = null!;
    public Order Order { get; private set; } = null!;

    public OrderItem(int quantity, decimal price, Guid productId, Guid orderId)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive.");
        if (productId == Guid.Empty)
            throw new ArgumentException("Product id is required.", nameof(productId));
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order id is required.", nameof(orderId));

        Quantity = quantity;
        Price = price;
        ProductId = productId;
        OrderId = orderId;
    }
}
