namespace Ecomm.Products.WebApi.Shared.Abstractions;

public interface IEventOutboxService
{
    Task AddAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent;
}
