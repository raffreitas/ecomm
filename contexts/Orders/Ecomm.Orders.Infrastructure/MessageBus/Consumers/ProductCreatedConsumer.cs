using System.Text.Json;

using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecomm.Orders.Infrastructure.MessageBus.Consumers;

public sealed class ProductCreatedConsumer : BackgroundService
{
    private const string ProductCreatedQueueName = "product.created";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionFactory _factory;

    public ProductCreatedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _factory = new ConnectionFactory
        {
            HostName = configuration["MessageBus:RabbitMQ:HostName"] ?? string.Empty,
            UserName = configuration["MessageBus:RabbitMQ:UserName"] ?? string.Empty,
            Password = configuration["MessageBus:RabbitMQ:Password"] ?? string.Empty,
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await _factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: ProductCreatedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var envelope = JsonSerializer.Deserialize<ProductCreatedEnvelope>(
                    eventArgs.Body.Span,
                    SerializerOptions);
                if (envelope is null
                    || envelope.Type != ProductCreatedQueueName
                    || envelope.Version != 1
                    || envelope.Id == Guid.Empty
                    || envelope.OccurredAtUtc == default
                    || envelope.Data is null)
                {
                    await channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);
                    return;
                }

                await PersistProductAsync(envelope.Data, stoppingToken);
                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (JsonException)
            {
                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
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

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private async Task PersistProductAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        if (!await repository.ExistsAsync(product.Id, cancellationToken))
        {
            try
            {
                await repository.CreateAsync(product, cancellationToken);
            }
            catch (DbUpdateException)
            {
                if (!await repository.ExistsAsync(product.Id, cancellationToken))
                {
                    throw;
                }
            }
        }
    }

    private sealed record ProductCreatedEnvelope(
        Guid Id,
        string Type,
        int Version,
        DateTimeOffset OccurredAtUtc,
        Product Data);
}
