namespace Ecomm.Payments.Application.Abstractions;

public interface IMessageBusService
{
    Task PublishAsync(string queue, byte[] message, CancellationToken cancellationToken = default);
}