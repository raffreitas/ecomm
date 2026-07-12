using RabbitMQ.Client;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class RabbitMqEventTransport : IEventTransport, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public RabbitMqEventTransport(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["MessageBus:RabbitMQ:HostName"] ?? string.Empty,
            UserName = configuration["MessageBus:RabbitMQ:UserName"] ?? string.Empty,
            Password = configuration["MessageBus:RabbitMQ:Password"] ?? string.Empty,
        };
    }

    public async Task PublishAsync(
        string destination,
        ReadOnlyMemory<byte> envelope,
        CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        await using var channel = await connection.CreateChannelAsync(channelOptions, cancellationToken);
        await channel.QueueDeclareAsync(
            queue: destination,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: destination,
            mandatory: true,
            basicProperties: properties,
            body: envelope,
            cancellationToken: cancellationToken);
    }

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is not { IsOpen: true })
            {
                if (_connection is not null)
                {
                    await _connection.DisposeAsync();
                }

                _connection = await _factory.CreateConnectionAsync(cancellationToken);
            }

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectionLock.Dispose();
    }
}
