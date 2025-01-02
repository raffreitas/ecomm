using System.Text;
using System.Text.Json;

using Ecomm.Orders.Application.Abstractions;
using Ecomm.Orders.Domain.Events;

using MediatR;


namespace Ecomm.Orders.Application.Orders.CreateOrder.Events;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private const string QueueName = "order.created";
    private readonly IMessageBusService _bus;

    public OrderCreatedEventHandler(IMessageBusService bus)
    {
        _bus = bus;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(notification);
        var body = Encoding.UTF8.GetBytes(json);
        await _bus.PublishAsync(QueueName, body, cancellationToken);
    }
}