using Ecomm.Catalog.Infrastructure.Persistence;
using Ecomm.Messaging;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class CatalogOutboxStore(IServiceScopeFactory scopeFactory) : IOutboxStore
{
    public async Task<IReadOnlyList<OutboxDispatchMessage>> ClaimAsync(
        Guid leaseId,
        DateTimeOffset now,
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var ids = await db.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null
                && message.NextAttemptAtUtc <= now
                && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
            .OrderBy(message => message.OccurredAtUtc)
            .Select(message => message.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (ids.Count == 0)
            return [];

        await db.OutboxMessages
            .Where(message => ids.Contains(message.Id)
                && message.ProcessedAtUtc == null
                && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.LeaseId, leaseId)
                .SetProperty(message => message.LeaseExpiresAtUtc, now + leaseDuration), cancellationToken);

        return await db.OutboxMessages
            .Where(message => message.LeaseId == leaseId && message.ProcessedAtUtc == null)
            .OrderBy(message => message.OccurredAtUtc)
            .Select(message => new OutboxDispatchMessage(
                message.Id,
                message.Type,
                message.Version,
                message.Destination,
                message.Payload,
                message.CorrelationId,
                message.CausationId,
                message.Attempts))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkProcessedAsync(Guid id, Guid leaseId, DateTimeOffset processedAtUtc, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.OutboxMessages.Where(message => message.Id == id && message.LeaseId == leaseId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.ProcessedAtUtc, processedAtUtc)
                .SetProperty(message => message.LeaseId, (Guid?)null)
                .SetProperty(message => message.LeaseExpiresAtUtc, (DateTimeOffset?)null)
                .SetProperty(message => message.LastError, (string?)null), cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, Guid leaseId, int attempts, DateTimeOffset nextAttemptAtUtc, string error, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.OutboxMessages.Where(message => message.Id == id && message.LeaseId == leaseId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.Attempts, attempts)
                .SetProperty(message => message.NextAttemptAtUtc, nextAttemptAtUtc)
                .SetProperty(message => message.LastError, error)
                .SetProperty(message => message.LeaseId, (Guid?)null)
                .SetProperty(message => message.LeaseExpiresAtUtc, (DateTimeOffset?)null), cancellationToken);
    }
}
