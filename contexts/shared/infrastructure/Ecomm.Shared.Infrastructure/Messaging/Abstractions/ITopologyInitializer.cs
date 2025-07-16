namespace Ecomm.Shared.Infrastructure.Messaging.Abstractions;

public interface ITopologyInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
