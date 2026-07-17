using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Ecomm.Messaging;
using Ecomm.Payments.Application.Commands.ProcessPayment;
using Ecomm.Payments.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Payments.Worker;

public sealed class OrderCreatedWorker(
    ServiceBusClient client,
    IServiceScopeFactory scopeFactory,
    ILogger<OrderCreatedWorker> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private ServiceBusProcessor? _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = client.CreateProcessor("orders-events", "payments-order-processing", new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5),
        });
        _processor.ProcessMessageAsync += ProcessAsync;
        _processor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Service Bus processor failed for {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        };
        await _processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        try
        {
            ValidateProperties(args.Message);
            var envelope = args.Message.Body.ToObjectFromJson<EventEnvelope<OrderCreated>>(JsonOptions)
                ?? throw new InvalidDataException("Envelope is required.");
            if (envelope.Id == Guid.Empty || envelope.Type != "orders.order-created.v1"
                || envelope.Version != 1 || envelope.Data is null)
                throw new InvalidDataException("Envelope metadata is invalid.");
            if (!Guid.TryParse(args.Message.MessageId, out var messageId) || messageId != envelope.Id)
                throw new InvalidDataException("MessageId must match envelope Id.");

            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
            await using var transaction = await db.Database.BeginTransactionAsync(args.CancellationToken);
            const string consumerName = "payments-order-processing";
            if (await db.InboxMessages.AnyAsync(
                    message => message.MessageId == envelope.Id && message.ConsumerName == consumerName,
                    args.CancellationToken))
            {
                await transaction.RollbackAsync(args.CancellationToken);
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            await sender.Send(new ProcessPaymentCommand(
                envelope.Data.OrderId,
                envelope.Data.CustomerName,
                envelope.Data.CustomerDocument,
                envelope.Data.CardHash,
                envelope.Data.Total), args.CancellationToken);
            db.InboxMessages.Add(new InboxMessage
            {
                MessageId = envelope.Id,
                ConsumerName = consumerName,
                ProcessedAtUtc = DateTimeOffset.UtcNow,
            });
            await db.SaveChangesAsync(args.CancellationToken);
            await transaction.CommitAsync(args.CancellationToken);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException exception)
        {
            await args.DeadLetterMessageAsync(args.Message, "InvalidJson", exception.Message);
        }
        catch (InvalidDataException exception)
        {
            await args.DeadLetterMessageAsync(args.Message, "InvalidMessage", exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Transient failure processing message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private static void ValidateProperties(ServiceBusReceivedMessage message)
    {
        if (!message.ApplicationProperties.TryGetValue("Type", out var type)
            || !string.Equals(type as string, "orders.order-created.v1", StringComparison.Ordinal))
            throw new InvalidDataException("Type application property is invalid.");
        if (!message.ApplicationProperties.TryGetValue("Version", out var version) || Convert.ToInt32(version) != 1)
            throw new InvalidDataException("Version application property must be 1.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
            await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }

    private sealed record OrderCreated(
        Guid OrderId,
        string CustomerName,
        string CustomerDocument,
        string CardHash,
        decimal Total);
}
