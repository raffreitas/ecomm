using Ecomm.Orders.Domain.Enums;
using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class Order : Entity
{
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
    public IList<OrderItem> Items { get; private set; } = [];

    public Order(decimal total)
    {
        Total = total;
        Status = OrderStatus.Pending;
    }
}