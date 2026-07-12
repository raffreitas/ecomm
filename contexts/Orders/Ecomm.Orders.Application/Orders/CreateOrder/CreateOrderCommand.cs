using MediatR;

namespace Ecomm.Orders.Application.Orders.CreateOrder;

public record CreateOrderCommand : IRequest
{
    public string CardHash { get; set; } = string.Empty;
    public IReadOnlyList<CreateOrderItem> Items { get; set; } = [];
}

public record CreateOrderItem
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
}