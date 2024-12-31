using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class OrderItem : Entity
{
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid OrderId { get; private set; }

    // EF. Rel
    public Product Product { get; private set; }
    public Order Order { get; private set; }

    public OrderItem(int quantity, decimal price, Guid productId, Guid orderId)
    {
        Quantity = quantity;
        Price = price;
        ProductId = productId;
        OrderId = orderId;
    }
}