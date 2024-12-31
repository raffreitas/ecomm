using MediatR;

namespace Ecomm.Orders.Application.Orders.CreateOrder;

public record CreateOrderCommand : IRequest
{
    public string CardHash { get; set; }
    public IList<CreateOrderItem> Items { get; set; }
}

public record CreateOrderItem
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
}