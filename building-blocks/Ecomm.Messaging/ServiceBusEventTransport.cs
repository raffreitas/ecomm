using Azure.Messaging.ServiceBus;

namespace Ecomm.Messaging;

public interface IEventTransport
{
    Task PublishAsync(OutboxDispatchMessage message, CancellationToken cancellationToken);
}

public sealed class ServiceBusEventTransport(ServiceBusClient client) : IEventTransport
{
    public async Task PublishAsync(OutboxDispatchMessage message, CancellationToken cancellationToken)
    {
        await using var sender = client.CreateSender(message.Destination);
        var serviceBusMessage = new ServiceBusMessage(BinaryData.FromString(message.Payload))
        {
            MessageId = message.Id.ToString(),
            ContentType = "application/json",
            CorrelationId = message.CorrelationId,
        };
        serviceBusMessage.ApplicationProperties["Type"] = message.Type;
        serviceBusMessage.ApplicationProperties["Version"] = message.Version;
        if (message.CausationId is not null)
            serviceBusMessage.ApplicationProperties["CausationId"] = message.CausationId;

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }
}
