using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.UnitTests;

internal static class TestDb
{
    public static CatalogDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CatalogDbContext(options);
    }
}

internal sealed class FakeMessageBus : IMessageBusService
{
    private readonly Func<bool>? _stateCheck;

    public FakeMessageBus(Func<bool>? stateCheck = null)
    {
        _stateCheck = stateCheck;
    }

    public bool ThrowOnPublish { get; init; }
    public bool WasStatePersistedAtPublish { get; private set; }
    public string? Queue { get; private set; }
    public byte[]? Message { get; private set; }

    public Task PublishAsync(string queue, byte[] message, CancellationToken cancellationToken = default)
    {
        WasStatePersistedAtPublish = _stateCheck?.Invoke() ?? false;
        Queue = queue;
        Message = message;

        if (ThrowOnPublish)
        {
            throw new InvalidOperationException("Broker unavailable");
        }

        return Task.CompletedTask;
    }
}
