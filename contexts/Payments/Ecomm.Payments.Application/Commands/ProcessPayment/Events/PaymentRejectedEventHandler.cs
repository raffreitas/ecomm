using System.Text;
using System.Text.Json;

using Ecomm.Payments.Application.Abstractions;
using Ecomm.Payments.Domain.Events;

using MediatR;

namespace Ecomm.Payments.Application.Commands.ProcessPayment.Events;
internal class PaymentRejectedEventHandler : INotificationHandler<PaymentRejectedEvent>
{
    private const string QueueName = "payment.rejected";
    private readonly IMessageBusService _bus;

    public PaymentRejectedEventHandler(IMessageBusService bus)
    {
        _bus = bus;
    }

    public async Task Handle(PaymentRejectedEvent notification, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(notification);
        var body = Encoding.UTF8.GetBytes(json);
        await _bus.PublishAsync(QueueName, body, cancellationToken);
    }
}
