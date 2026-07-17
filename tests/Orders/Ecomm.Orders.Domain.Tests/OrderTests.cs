using Ecomm.Orders.Domain.DTOs;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Enums;

using Xunit;

namespace Ecomm.Orders.Domain.Tests;

public sealed class OrderTests
{
    [Fact]
    public void Create_calculates_total_and_starts_pending()
    {
        var order = CreateOrder((Guid.NewGuid(), 2, 12.50m), (Guid.NewGuid(), 1, 5m));

        Assert.Equal(30m, order.Total);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(2, order.Items.Count);
    }

    [Fact]
    public void Create_rejects_empty_items()
    {
        var dto = new CreateOrderDto(new Customer("Ada", "123"), "card", []);

        Assert.Throws<ArgumentException>(() => Order.Create(dto));
    }

    [Fact]
    public void Create_rejects_duplicate_products()
    {
        var productId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => CreateOrder((productId, 1, 10m), (productId, 2, 10m)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_rejects_non_positive_quantity(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrder((Guid.NewGuid(), quantity, 10m)));
    }

    [Fact]
    public void Paid_order_cannot_transition_again()
    {
        var order = CreateOrder((Guid.NewGuid(), 1, 10m));
        order.MarkAsPaid();

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Throws<InvalidOperationException>(order.MarkAsFailed);
    }

    private static Order CreateOrder(params (Guid ProductId, int Quantity, decimal Price)[] items)
    {
        var dto = new CreateOrderDto(
            new Customer("Ada", "123"),
            "card",
            items.Select(item => new CreateOrderItemDto(item.ProductId, item.Quantity, item.Price)));
        return Order.Create(dto);
    }
}
