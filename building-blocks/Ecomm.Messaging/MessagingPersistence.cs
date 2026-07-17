using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Messaging;

public static class MessagingPersistence
{
    public static ModelBuilder AddMessagingEntities(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(message => message.Id);
            builder.Property(message => message.Type).HasMaxLength(200).IsRequired();
            builder.Property(message => message.Destination).HasMaxLength(200).IsRequired();
            builder.Property(message => message.Payload).IsRequired();
            builder.Property(message => message.CorrelationId).HasMaxLength(100);
            builder.Property(message => message.CausationId).HasMaxLength(100);
            builder.Property(message => message.LastError).HasMaxLength(2000);
            builder.HasIndex(message => new { message.ProcessedAtUtc, message.NextAttemptAtUtc });
            builder.HasIndex(message => message.LeaseId);
        });

        modelBuilder.Entity<InboxMessage>(builder =>
        {
            builder.ToTable("InboxMessages");
            builder.HasKey(message => new { message.MessageId, message.ConsumerName });
            builder.Property(message => message.ConsumerName).HasMaxLength(200);
        });

        return modelBuilder;
    }
}

public sealed class EfOutboxStore<TDbContext>(IServiceScopeFactory scopeFactory) : IOutboxStore
    where TDbContext : DbContext
{
    public async Task<IReadOnlyList<OutboxDispatchMessage>> ClaimAsync(Guid leaseId, DateTimeOffset now, int batchSize, TimeSpan leaseDuration, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var set = db.Set<OutboxMessage>();
        var ids = await set
            .Where(message => message.ProcessedAtUtc == null
                && message.NextAttemptAtUtc <= now
                && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
            .OrderBy(message => message.OccurredAtUtc)
            .Select(message => message.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (ids.Count == 0)
            return [];

        await set.Where(message => ids.Contains(message.Id)
                && message.ProcessedAtUtc == null
                && (message.LeaseExpiresAtUtc == null || message.LeaseExpiresAtUtc <= now))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.LeaseId, leaseId)
                .SetProperty(message => message.LeaseExpiresAtUtc, now + leaseDuration), cancellationToken);

        return await set.Where(message => message.LeaseId == leaseId && message.ProcessedAtUtc == null)
            .OrderBy(message => message.OccurredAtUtc)
            .Select(message => new OutboxDispatchMessage(message.Id, message.Type, message.Version,
                message.Destination, message.Payload, message.CorrelationId, message.CausationId, message.Attempts))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkProcessedAsync(Guid id, Guid leaseId, DateTimeOffset processedAtUtc, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Set<OutboxMessage>().Where(message => message.Id == id && message.LeaseId == leaseId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.ProcessedAtUtc, processedAtUtc)
                .SetProperty(message => message.LeaseId, (Guid?)null)
                .SetProperty(message => message.LeaseExpiresAtUtc, (DateTimeOffset?)null)
                .SetProperty(message => message.LastError, (string?)null), cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, Guid leaseId, int attempts, DateTimeOffset nextAttemptAtUtc, string error, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Set<OutboxMessage>().Where(message => message.Id == id && message.LeaseId == leaseId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.Attempts, attempts)
                .SetProperty(message => message.NextAttemptAtUtc, nextAttemptAtUtc)
                .SetProperty(message => message.LastError, error)
                .SetProperty(message => message.LeaseId, (Guid?)null)
                .SetProperty(message => message.LeaseExpiresAtUtc, (DateTimeOffset?)null), cancellationToken);
    }
}
