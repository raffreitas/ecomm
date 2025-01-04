using System.Text;
using System.Text.Json;

using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecomm.Orders.Infrastructure.MessageBus.Consumers;

public class ProductCreatedConsumer : BackgroundService
{
    private const string ProductCreatedQueueName = "product.created";
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionFactory _factory;

    public ProductCreatedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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
            queue: ProductCreatedQueueName,
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
                var productBytesArray = eventArgs.Body.ToArray();
                var createProductJson = Encoding.UTF8.GetString(productBytesArray);

                Console.WriteLine($"Received: {createProductJson}");
                var product = JsonSerializer.Deserialize<Product>(createProductJson);

                if (product is not null)
                    await PersistProductAsync(product, stoppingToken);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch
            {
                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: ProductCreatedQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task PersistProductAsync(Product product,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var productRepository = scope.ServiceProvider.GetService<IProductRepository>();
        await productRepository!.CreateAsync(product, cancellationToken);
    }
}