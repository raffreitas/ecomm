using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;

public sealed class IntegrationEventMessageMapper : IIntegrationEventMessageMapper
{
    public IMessage Map(IntegrationEvent integrationEvent)
    {
        return new IntegrationEventMessageAdapter(integrationEvent);
    }
}
