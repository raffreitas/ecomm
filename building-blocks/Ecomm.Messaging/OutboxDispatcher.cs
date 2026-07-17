using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecomm.Messaging;

public sealed class OutboxDispatcher(
    IOutboxStore store,
    IEventTransport transport,
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
                if (await DispatchBatchAsync(stoppingToken) == 0)
                    await Task.Delay(_options.PollingInterval, timeProvider, stoppingToken);
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

    public async Task<int> DispatchBatchAsync(CancellationToken cancellationToken)
    {
        var leaseId = Guid.NewGuid();
        var messages = await store.ClaimAsync(
            leaseId,
            timeProvider.GetUtcNow(),
            _options.BatchSize,
            _options.LeaseDuration,
            cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await transport.PublishAsync(message, cancellationToken);
                await store.MarkProcessedAsync(message.Id, leaseId, timeProvider.GetUtcNow(), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                var attempts = message.Attempts + 1;
                await store.MarkFailedAsync(
                    message.Id,
                    leaseId,
                    attempts,
                    timeProvider.GetUtcNow() + RetryDelay(attempts),
                    exception.Message.Length <= 2000 ? exception.Message : exception.Message[..2000],
                    cancellationToken);
                logger.LogWarning(exception, "Failed to publish outbox event {EventId} {EventType}", message.Id, message.Type);
            }
        }

        return messages.Count;
    }

    private TimeSpan RetryDelay(int attempts)
    {
        var multiplier = Math.Pow(2, Math.Min(attempts - 1, 30));
        return TimeSpan.FromTicks((long)Math.Min(
            _options.InitialRetryDelay.Ticks * multiplier,
            _options.MaximumRetryDelay.Ticks));
    }
}
