using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;

public class IntegrationEventMessageAdapter(IntegrationEvent integrationEvent) : IMessage
{
    public IntegrationEvent IntegrationEvent { get; } = integrationEvent;
    public string MessageType => IntegrationEvent.GetType().Name;
}
