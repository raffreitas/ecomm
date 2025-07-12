using System.Text.Json;

using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;
using Ecomm.Shared.Infrastructure.Observability.Correlation.Factory;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Shared.Workers;

public sealed class OutboxEventPublisherWorker(
    IServiceProvider serviceProvider,
    ILogger<OutboxEventPublisherWorker> logger,
    IMessagePublisher messagePublisher,
    ICorrelationContextFactory correlationContextFactory) : BackgroundService
{
    private const int DelayBetweenRetriesInSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var eventMessageMapper = scope.ServiceProvider.GetRequiredService<IIntegrationEventMessageMapper>();
        var topologyInitializer = scope.ServiceProvider.GetRequiredService<ITopologyInitializer>();

        await topologyInitializer.InitializeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var pendingEvents = await dbContext.Set<OutboxEvent>()
                .Where(e => e.ProcessedAt == null && e.RetryCount > 0)
                .OrderBy(e => e.CreatedAt)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var outboxEvent in pendingEvents)
            {
                correlationContextFactory.Create(outboxEvent.CorrelationId);
                try
                {
                    var eventType = Type.GetType(outboxEvent.Type ?? "");
                    if (eventType is null || !typeof(IntegrationEvent).IsAssignableFrom(eventType))
                        throw new InvalidOperationException($"Type {outboxEvent.Type} is not a valid IntegrationEvent.");

                    var integrationEvent = (IntegrationEvent?)JsonSerializer.Deserialize(outboxEvent.Content, eventType);
                    if (integrationEvent is null)
                        throw new InvalidOperationException($"Failed to deserialize event content for type {outboxEvent.Type}.");

                    var message = eventMessageMapper.Map(integrationEvent);
                    await messagePublisher.PublishAsync(message, stoppingToken);

                    outboxEvent.MarkProcessed();
                    logger.LogInformation("Published event {EventType} with id {EventId} via IMessagePublisher.", outboxEvent.Type, outboxEvent.Id);
                }
                catch (Exception ex)
                {
                    outboxEvent.MarkError(ex.Message);
                    logger.LogError(ex, "Failed to publish event {EventType} with id {EventId}.", outboxEvent.Type, outboxEvent.Id);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(DelayBetweenRetriesInSeconds), stoppingToken);
        }
    }
}
