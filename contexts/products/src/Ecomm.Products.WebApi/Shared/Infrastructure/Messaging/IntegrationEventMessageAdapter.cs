using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;

public sealed class IntegrationEventMessageAdapter(IntegrationEvent integrationEvent) : IMessage
{
    public string MessageType => integrationEvent.GetType().Name;
    public object Payload => integrationEvent;
}
