using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Ecomm.Messaging;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Worker;

public sealed class OrdersEventsWorker(
    ServiceBusClient client,
    IServiceScopeFactory scopeFactory,
    ILogger<OrdersEventsWorker> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly List<ServiceBusProcessor> _processors = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processors.Add(CreateProcessor("catalog-events", "orders-product-projection"));
        _processors.Add(CreateProcessor("customers-events", "orders-customer-projection"));
        _processors.Add(CreateProcessor("payments-events", "orders-payment-status"));
        foreach (var processor in _processors)
            await processor.StartProcessingAsync(stoppingToken);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private ServiceBusProcessor CreateProcessor(string topic, string subscription)
    {
        var processor = client.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 4,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5),
        });
        processor.ProcessMessageAsync += args => ProcessAsync(subscription, args);
        processor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Service Bus processor failed for {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        };
        return processor;
    }

    private async Task ProcessAsync(string consumerName, ProcessMessageEventArgs args)
    {
        try
        {
            var type = GetType(args.Message);
            if (type == "catalog.product-created.v1")
                await ProcessEnvelopeAsync<ProductCreated>(consumerName, type, args, ApplyProductAsync);
            else if (type == "customers.customer-created.v1")
                await ProcessEnvelopeAsync<CustomerCreated>(consumerName, type, args, ApplyCustomerAsync);
            else if (type == "payments.payment-approved.v1")
                await ProcessEnvelopeAsync<PaymentApproved>(consumerName, type, args, ApplyPaymentApprovedAsync);
            else if (type == "payments.payment-rejected.v1")
                await ProcessEnvelopeAsync<PaymentRejected>(consumerName, type, args, ApplyPaymentRejectedAsync);
            else
                throw new InvalidDataException($"Unsupported event type '{type}'.");
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

    private async Task ProcessEnvelopeAsync<TData>(
        string consumerName,
        string expectedType,
        ProcessMessageEventArgs args,
        Func<OrdersDbContext, TData, CancellationToken, Task> apply)
    {
        var envelope = args.Message.Body.ToObjectFromJson<EventEnvelope<TData>>(JsonOptions)
            ?? throw new InvalidDataException("Envelope is required.");
        if (envelope.Id == Guid.Empty || envelope.Type != expectedType || envelope.Version != 1 || envelope.Data is null)
            throw new InvalidDataException("Envelope metadata is invalid.");
        if (!Guid.TryParse(args.Message.MessageId, out var messageId) || messageId != envelope.Id)
            throw new InvalidDataException("MessageId must match envelope Id.");

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync(args.CancellationToken);
        if (await db.InboxMessages.AnyAsync(
                message => message.MessageId == envelope.Id && message.ConsumerName == consumerName,
                args.CancellationToken))
        {
            await transaction.RollbackAsync(args.CancellationToken);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        await apply(db, envelope.Data, args.CancellationToken);
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

    private static string GetType(ServiceBusReceivedMessage message)
    {
        if (!message.ApplicationProperties.TryGetValue("Type", out var value) || value is not string type)
            throw new InvalidDataException("Type application property is required.");
        if (!message.ApplicationProperties.TryGetValue("Version", out var version) || Convert.ToInt32(version) != 1)
            throw new InvalidDataException("Version application property must be 1.");
        return type;
    }

    private static async Task ApplyProductAsync(OrdersDbContext db, ProductCreated data, CancellationToken token)
    {
        if (!await db.Products.AnyAsync(product => product.Id == data.Id, token))
            db.Products.Add(Product.CreateSnapshot(data.Id, data.Name, data.Price));
    }

    private static async Task ApplyCustomerAsync(OrdersDbContext db, CustomerCreated data, CancellationToken token)
    {
        if (!await db.Customers.AnyAsync(customer => customer.Id == data.Id, token))
            db.Customers.Add(Customer.CreateSnapshot(data.Id, data.Name, data.Document));
    }

    private static async Task ApplyPaymentApprovedAsync(OrdersDbContext db, PaymentApproved data, CancellationToken token)
    {
        var order = await db.Orders.SingleOrDefaultAsync(order => order.Id == data.OrderId, token)
            ?? throw new InvalidOperationException($"Order {data.OrderId} was not found.");
        order.MarkAsPaid();
    }

    private static async Task ApplyPaymentRejectedAsync(OrdersDbContext db, PaymentRejected data, CancellationToken token)
    {
        var order = await db.Orders.SingleOrDefaultAsync(order => order.Id == data.OrderId, token)
            ?? throw new InvalidOperationException($"Order {data.OrderId} was not found.");
        order.MarkAsFailed();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
            await processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }

    private sealed record ProductCreated(Guid Id, string Name, string Description, decimal Price, string ImageUrl, Guid CategoryId);
    private sealed record CustomerCreated(Guid Id, string Name, string Email, string Document);
    private sealed record PaymentApproved(Guid OrderId, Guid PaymentId, string ExternalPaymentId);
    private sealed record PaymentRejected(Guid OrderId, Guid PaymentId, string? ExternalPaymentId, string Reason);
}
