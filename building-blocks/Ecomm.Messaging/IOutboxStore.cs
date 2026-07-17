namespace Ecomm.Messaging;

public interface IOutboxStore
{
    Task<IReadOnlyList<OutboxDispatchMessage>> ClaimAsync(
        Guid leaseId,
        DateTimeOffset now,
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken);

    Task MarkProcessedAsync(Guid id, Guid leaseId, DateTimeOffset processedAtUtc, CancellationToken cancellationToken);

    Task MarkFailedAsync(
        Guid id,
        Guid leaseId,
        int attempts,
        DateTimeOffset nextAttemptAtUtc,
        string error,
        CancellationToken cancellationToken);
}
