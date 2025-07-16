using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Ecomm.Shared.Infrastructure.Messaging.Abstractions;
using Ecomm.Shared.Infrastructure.Observability.Correlation.Factory;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecomm.Shared.Infrastructure.Messaging.Providers.RabbitMQ;
internal sealed class RabbitMqMessageConsumer(
    ChannelFactory channelFactory,
    IOptions<RabbitMqMessageSettings> options,
    ICorrelationContextFactory correlationContextFactory,
    ILogger<RabbitMqMessageConsumer> logger
) : IMessageConsumer
{
    private readonly RabbitMqMessageSettings _settings = options.Value;

    public async Task ConsumeAsync<TMessage>(Func<TMessage, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var messageType = typeof(TMessage).Name;
        var messageSettings = _settings.GetMessageSettings(messageType);

        var channel = await channelFactory.CreateForConsumerAsync(cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, @event) =>
        {
            var parentContext = MessagingDiagnostics.Propagator.Extract(default,
                 @event.BasicProperties,
                 ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            var activityName = $"{@event.RoutingKey} consume";
            using var activity = MessagingDiagnostics.ActivitySource.StartActivity(activityName, ActivityKind.Consumer,
                parentContext.ActivityContext);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination", messageSettings.QueueName);
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.operation", "consume");
            activity?.SetTag("messaging.message_type", messageType);
            activity?.SetTag("messaging.correlation_id", @event.BasicProperties.CorrelationId);

            try
            {
                var jsonMessage = Encoding.UTF8.GetString(@event.Body.Span);
                var message = JsonSerializer.Deserialize<TMessage>(jsonMessage);

                if (message is not null)
                {
                    correlationContextFactory.Create(@event.BasicProperties.CorrelationId ?? Guid.NewGuid().ToString("N"));
                    await handler(message, cancellationToken);
                    await channel.BasicAckAsync(@event.DeliveryTag, false, cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                else
                {
                    logger.LogWarning("Failed to deserialize message of type {MessageType}", messageType);
                    await channel.BasicRejectAsync(@event.DeliveryTag, false, cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Error, "Failed to deserialize message");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message of type {MessageType}", messageType);
                await channel.BasicRejectAsync(@event.DeliveryTag, requeue: false, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddException(ex);
            }
        };

        await channel.BasicConsumeAsync(
            queue: messageSettings.QueueName,
            autoAck: false,
            consumer,
            cancellationToken
        );

        logger.LogInformation("Subscribed to queue {QueueName} for message type {MessageType}", messageSettings.QueueName, messageType);
    }

    private static IEnumerable<string> ExtractTraceContextFromBasicProperties(IReadOnlyBasicProperties props, string key)
    {
        if (!props.Headers!.TryGetValue(key, out var value))
            return [];

        return value is not byte[] bytes
            ? []
            : [Encoding.UTF8.GetString(bytes)];
    }
}
