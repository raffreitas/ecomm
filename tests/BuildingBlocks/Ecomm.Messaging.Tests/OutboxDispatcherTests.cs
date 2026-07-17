using Ecomm.Messaging;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Xunit;

namespace Ecomm.Messaging.Tests;

public sealed class OutboxDispatcherTests
{
    [Fact]
    public async Task Dispatch_marks_successful_message_as_processed()
    {
        var store = new FakeStore(Message());
        var dispatcher = CreateDispatcher(store, new FakeTransport());

        var count = await dispatcher.DispatchBatchAsync(CancellationToken.None);

        Assert.Equal(1, count);
        Assert.Equal(store.Message.Id, store.ProcessedId);
        Assert.Null(store.FailedId);
    }

    [Fact]
    public async Task Dispatch_schedules_retry_when_transport_fails()
    {
        var store = new FakeStore(Message());
        var dispatcher = CreateDispatcher(store, new FakeTransport(new InvalidOperationException("offline")));

        var count = await dispatcher.DispatchBatchAsync(CancellationToken.None);

        Assert.Equal(1, count);
        Assert.Equal(store.Message.Id, store.FailedId);
        Assert.Equal(1, store.FailedAttempts);
        Assert.Null(store.ProcessedId);
    }

    [Fact]
    public async Task Dispatch_propagates_requested_cancellation_without_retry()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var store = new FakeStore(Message());
        var dispatcher = CreateDispatcher(store, new FakeTransport());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => dispatcher.DispatchBatchAsync(source.Token));

        Assert.Null(store.FailedId);
        Assert.Null(store.ProcessedId);
    }

    private static OutboxDispatcher CreateDispatcher(FakeStore store, IEventTransport transport) => new(
        store,
        transport,
        Options.Create(new OutboxOptions()),
        TimeProvider.System,
        NullLogger<OutboxDispatcher>.Instance);

    private static OutboxDispatchMessage Message() => new(
        Guid.NewGuid(), "test.v1", 1, "test-events", "{}", "correlation", "causation", 0);

    private sealed class FakeStore(OutboxDispatchMessage message) : IOutboxStore
    {
        public OutboxDispatchMessage Message { get; } = message;
        public Guid? ProcessedId { get; private set; }
        public Guid? FailedId { get; private set; }
        public int FailedAttempts { get; private set; }

        public Task<IReadOnlyList<OutboxDispatchMessage>> ClaimAsync(
            Guid leaseId, DateTimeOffset now, int batchSize, TimeSpan leaseDuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IReadOnlyList<OutboxDispatchMessage>>([Message]);
        }

        public Task MarkProcessedAsync(Guid id, Guid leaseId, DateTimeOffset processedAtUtc, CancellationToken cancellationToken)
        {
            ProcessedId = id;
            return Task.CompletedTask;
        }

        public Task MarkFailedAsync(Guid id, Guid leaseId, int attempts, DateTimeOffset nextAttemptAtUtc, string error, CancellationToken cancellationToken)
        {
            FailedId = id;
            FailedAttempts = attempts;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTransport(Exception? exception = null) : IEventTransport
    {
        public Task PublishAsync(OutboxDispatchMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return exception is null ? Task.CompletedTask : Task.FromException(exception);
        }
    }
}
