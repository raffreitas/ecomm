using RabbitMQ.Client;

namespace Ecomm.Shared.Infrastructure.Messaging.Providers.RabbitMQ;

internal sealed class ConnectionProvider(IConnection consumerConnection, IConnection producerConnection)
{
    public IConnection ConsumerConnection { get; } = consumerConnection;
    public IConnection ProducerConnection { get; } = producerConnection;
}