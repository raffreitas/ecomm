using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;

public interface IIntegrationEventMessageMapper
{
    IMessage Map(IntegrationEvent integrationEvent);
}
