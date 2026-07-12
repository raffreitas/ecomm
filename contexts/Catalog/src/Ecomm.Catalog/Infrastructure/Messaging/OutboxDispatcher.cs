using System.Text;

using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    IEventTransport eventTransport,
    IOptions<OutboxOptions> options,
    TimeProvider timeProvider,
    ILogger<OutboxDispatcher> logger) : BackgroundService
{
    private readonly OutboxOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await DispatchBatchAsync(stoppingToken);
                if (processed == 0)
                {
                    await Task.Delay(_options.PollingInterval, timeProvider, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Outbox dispatch cycle failed");
                await Task.Delay(_options.PollingInterval, timeProvider, stoppingToken);
            }
        }
    }

    internal async Task<int> DispatchBatchAsync(CancellationToken cancellationToken)
    {
        var leaseId = Guid.NewGuid();
        var now = timeProvider.GetUtcNow();

        await using (var claimScope = scopeFactory.CreateAsyncScope())
        {
            var dbContext = claimScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var candidateIds = await dbContext.OutboxMessages
                .Where(message => message.ProcessedAtUtc == null
                    && message.NextAttemptAtUtc <= now
                    && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
                .OrderBy(message => message.OccurredAtUtc)
                .Select(message => message.Id)
                .Take(_options.BatchSize)
                .ToListAsync(cancellationToken);

            if (candidateIds.Count == 0)
            {
                return 0;
            }

            await dbContext.OutboxMessages
                .Where(message => candidateIds.Contains(message.Id)
                    && message.ProcessedAtUtc == null
                    && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(message => message.LeaseId, leaseId)
                    .SetProperty(message => message.LeaseExpiresAtUtc, now + _options.LeaseDuration),
                    cancellationToken);
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var processingContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var messages = await processingContext.OutboxMessages
            .Where(message => message.LeaseId == leaseId && message.ProcessedAtUtc == null)
            .OrderBy(message => message.OccurredAtUtc)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await eventTransport.PublishAsync(
                    message.Destination,
                    Encoding.UTF8.GetBytes(message.Payload),
                    cancellationToken);

                message.ProcessedAtUtc = timeProvider.GetUtcNow();
                message.LeaseId = null;
                message.LeaseExpiresAtUtc = null;
                message.LastError = null;
                logger.LogInformation(
                    "Published outbox event {EventId} {EventType} to {Destination} after {Attempts} attempts",
                    message.Id,
                    message.Type,
                    message.Destination,
                    message.Attempts);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                message.Attempts++;
                message.LastError = exception.Message.Length <= 2000
                    ? exception.Message
                    : exception.Message[..2000];
                message.NextAttemptAtUtc = timeProvider.GetUtcNow() + GetRetryDelay(message.Attempts);
                message.LeaseId = null;
                message.LeaseExpiresAtUtc = null;
                logger.LogWarning(
                    exception,
                    "Failed to publish outbox event {EventId} {EventType} to {Destination}; attempt {Attempt}",
                    message.Id,
                    message.Type,
                    message.Destination,
                    message.Attempts);
            }

            await processingContext.SaveChangesAsync(cancellationToken);
        }

        return messages.Count;
    }

    private TimeSpan GetRetryDelay(int attempts)
    {
        var multiplier = Math.Pow(2, Math.Min(attempts - 1, 30));
        var delay = TimeSpan.FromTicks((long)Math.Min(
            _options.InitialRetryDelay.Ticks * multiplier,
            _options.MaximumRetryDelay.Ticks));
        return delay;
    }
}
