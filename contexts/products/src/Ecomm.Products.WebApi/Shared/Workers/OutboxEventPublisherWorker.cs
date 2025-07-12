using System.Text.Json;

using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;
using Ecomm.Shared.Infrastructure.Messaging.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Products.WebApi.Shared.Workers;

public class OutboxEventPublisherWorker(
    IServiceProvider serviceProvider,
    ILogger<OutboxEventPublisherWorker> logger,
    IMessagePublisher messagePublisher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var eventMessageMapper = scope.ServiceProvider.GetRequiredService<IIntegrationEventMessageMapper>();

            var pendingEvents = await dbContext.Set<OutboxEvent>()
                .Where(e => e.ProcessedAt == null && e.RetryCount > 0)
                .OrderBy(e => e.CreatedAt)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var outboxEvent in pendingEvents)
            {
                try
                {
                    var eventType = Type.GetType(outboxEvent.Type ?? "");
                    if (eventType == null || !typeof(IntegrationEvent).IsAssignableFrom(eventType))
                        throw new InvalidOperationException($"Type {outboxEvent.Type} is not a valid IntegrationEvent.");

                    var integrationEvent = (IntegrationEvent?)JsonSerializer.Deserialize(outboxEvent.Content, eventType);
                    if (integrationEvent == null)
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
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
