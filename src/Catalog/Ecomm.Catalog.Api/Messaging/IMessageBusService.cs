namespace Ecomm.Catalog.Messaging;

public interface IMessageBusService
{
    Task PublishAsync(string queue, byte[] message, CancellationToken cancellationToken = default);
}