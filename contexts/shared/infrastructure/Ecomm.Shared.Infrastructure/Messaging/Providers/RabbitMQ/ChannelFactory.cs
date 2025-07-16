using RabbitMQ.Client;

namespace Ecomm.Shared.Infrastructure.Messaging.Providers.RabbitMQ;

internal sealed class ChannelFactory : IAsyncDisposable
{
    private readonly IConnection _consumerConnection;
    private readonly IConnection _producerConnection;
    private readonly ThreadLocal<IChannel?> _consumerCache = new(true);
    private readonly ThreadLocal<IChannel?> _producerCache = new(true);

    private ChannelFactory(IConnection consumerConnection, IConnection producerConnection)
    {
        _consumerConnection = consumerConnection;
        _producerConnection = producerConnection;
    }

    public static async Task<ChannelFactory> CreateAsync(ConnectionFactory connectionFactory)
    {
        var consumerConnection = await connectionFactory.CreateConnectionAsync("ecomm-consumer");
        var producerConnection = await connectionFactory.CreateConnectionAsync("ecomm-producer");
        return new ChannelFactory(consumerConnection, producerConnection);
    }

    public async Task<IChannel> CreateForProducerAsync(CancellationToken ct = default)
        => await CreateAsync(_producerConnection, _producerCache, ct);

    public async Task<IChannel> CreateForConsumerAsync(CancellationToken ct = default)
        => await CreateAsync(_consumerConnection, _consumerCache, ct);

    private static async Task<IChannel> CreateAsync(IConnection connection, ThreadLocal<IChannel?> channelCache, CancellationToken ct)
    {
        if (channelCache.Value is not null)
            return channelCache.Value;

        var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        channelCache.Value = channel;
        return channel;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var channel in _consumerCache.Values)
            if (channel is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                channel?.Dispose();

        foreach (var channel in _producerCache.Values)
            if (channel is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                channel?.Dispose();

        _consumerCache.Dispose();
        _producerCache.Dispose();
        await _consumerConnection.DisposeAsync();
        await _producerConnection.DisposeAsync();
    }
}
