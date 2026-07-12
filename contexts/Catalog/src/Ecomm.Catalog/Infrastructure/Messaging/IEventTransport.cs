namespace Ecomm.Catalog.Infrastructure.Messaging;

public interface IEventTransport
{
    Task PublishAsync(
        string destination,
        ReadOnlyMemory<byte> envelope,
        CancellationToken cancellationToken = default);
}
