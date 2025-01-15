using System.Text;
using System.Text.Json;

using Ecomm.Orders.Application.Orders.MarkOrderAsPaid;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecomm.Orders.Infrastructure.MessageBus.Consumers;
internal class PaymentApprovedConsumer : BackgroundService
{
    private const string CustomerCreatedQueueName = "payment.approved";
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionFactory _factory;

    public PaymentApprovedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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
            queue: CustomerCreatedQueueName,
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
                var customerBytesArray = eventArgs.Body.ToArray();
                var createCustomerJson = Encoding.UTF8.GetString(customerBytesArray);

                var command = JsonSerializer.Deserialize<MarkOrderAsPaidCommand>(createCustomerJson);

                if (command is not null)
                    await SendCommand(command, stoppingToken);

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
            queue: CustomerCreatedQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task SendCommand(MarkOrderAsPaidCommand command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetService<ISender>();
        await sender!.Send(command, cancellationToken);
    }
}
