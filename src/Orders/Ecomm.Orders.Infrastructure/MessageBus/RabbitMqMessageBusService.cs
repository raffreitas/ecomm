using Ecomm.Orders.Application.Abstractions;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

namespace Ecomm.Orders.Infrastructure.MessageBus;

public class RabbitMqMessageBusService : IMessageBusService
{
    private readonly ConnectionFactory _factory;

    public RabbitMqMessageBusService(IConfiguration configuration)
    {
        var host = configuration.GetSection("MessageBus:RabbitMQ:HostName").Value ?? string.Empty;
        var userName = configuration.GetSection("MessageBus:RabbitMQ:UserName").Value ?? string.Empty;
        var password = configuration.GetSection("MessageBus:RabbitMQ:Password").Value ?? string.Empty;

        _factory = new ConnectionFactory { HostName = host, UserName = userName, Password = password };
    }

    public async Task PublishAsync(string queue, byte[] message, CancellationToken cancellationToken = default)
    {
        await using var connection = await _factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var props = new BasicProperties { ContentType = "application/json", };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: message,
            cancellationToken: cancellationToken);
    }
}