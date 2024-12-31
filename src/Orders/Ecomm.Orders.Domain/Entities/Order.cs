using Ecomm.Orders.Domain.DTOs;
using Ecomm.Orders.Domain.Enums;
using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class Order : Entity
{
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
    public IList<OrderItem> Items { get; private set; } = [];

    private Order()
    {
        Status = OrderStatus.Pending;
    }

    public static Order Create(CreateOrderDto createOrderDto)
    {
        ArgumentNullException.ThrowIfNull(createOrderDto);

        var order = new Order();

        foreach (var item in createOrderDto.Items)
        {
            order.AddItem(new OrderItem(item.Quantity, item.UnitPrice, item.ProductId, order.Id));
        }

        return order;
    }

    private void AddItem(OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem);

        Items.Add(orderItem);

        CalculateOrderTotal();
    }

    private void CalculateOrderTotal()
    {
        Total = Items.Sum(item => item.Price * item.Quantity);
    }
}