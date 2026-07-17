using Ecomm.Orders.Domain.DTOs;
using Ecomm.Orders.Domain.Enums;
using Ecomm.Orders.Domain.Events;
using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class Order : Entity
{
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
    public IList<OrderItem> Items { get; private set; } = [];

    public Customer Customer { get; private set; } = null!;

    private Order(Guid customerId)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
    }

    public static Order Create(CreateOrderDto createOrderDto)
    {
        ArgumentNullException.ThrowIfNull(createOrderDto);

        if (createOrderDto.Customer is null)
            throw new ArgumentException("Customer is required.", nameof(createOrderDto));

        if (string.IsNullOrWhiteSpace(createOrderDto.CardHash))
            throw new ArgumentException("Card hash is required.", nameof(createOrderDto));

        var items = createOrderDto.Items?.ToArray()
            ?? throw new ArgumentException("Items are required.", nameof(createOrderDto));

        if (items.Length == 0)
            throw new ArgumentException("An order must contain at least one item.", nameof(createOrderDto));

        if (items.Select(item => item.ProductId).Distinct().Count() != items.Length)
            throw new ArgumentException("An order cannot contain the same product more than once.", nameof(createOrderDto));

        var order = new Order(createOrderDto.Customer.Id);

        foreach (var item in items)
        {
            order.AddItem(new OrderItem(item.Quantity, item.UnitPrice, item.ProductId, order.Id));
        }

        order.AddDomainEvent(new OrderCreatedDomainEvent(
            order.Id,
            createOrderDto.Customer.Name,
            createOrderDto.Customer.Document,
            createOrderDto.CardHash, order.Total));

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

    public void MarkAsPaid()
    {
        EnsurePending();
        Status = OrderStatus.Paid;
    }

    public void MarkAsFailed()
    {
        EnsurePending();
        Status = OrderStatus.Failed;
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Order {Id} cannot transition from {Status}.");
    }
}
