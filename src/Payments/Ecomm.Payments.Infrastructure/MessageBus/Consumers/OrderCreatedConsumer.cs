using System.Text;
using System.Text.Json;

using Ecomm.Payments.Application.Commands;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecomm.Payments.Infrastructure.MessageBus.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private const string OrderCreatedQueueName = "order.created";
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionFactory _factory;

    public OrderCreatedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var host = configuration.GetSection("MessageBus:RabbitMQ:HostName").Value ?? string.Empty;
        var userName = configuration.GetSection("MessageBus:RabbitMQ:UserName").Value ?? string.Empty;
        var password = configuration.GetSection("MessageBus:RabbitMQ:Password").Value ?? string.Empty;

        _serviceProvider = serviceProvider;
        _factory = new ConnectionFactory { HostName = host, UserName = userName, Password = password };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await _factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: OrderCreatedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var orderBytesArray = eventArgs.Body.ToArray();
                var processPaymentDtoJson = Encoding.UTF8.GetString(orderBytesArray);
                Console.WriteLine($"Received: {processPaymentDtoJson}");
                var processPaymentDto = JsonSerializer.Deserialize<ProcessPaymentCommand>(processPaymentDtoJson);

                if (processPaymentDto is not null)
                    await ProcessPaymentAsync(processPaymentDto, cancellationToken: stoppingToken);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch
            {
                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: OrderCreatedQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task ProcessPaymentAsync(ProcessPaymentCommand processPaymentCommand,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetService<ISender>();
        await sender?.Send(processPaymentCommand, cancellationToken)!;
    }
}