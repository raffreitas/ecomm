using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Shared.Application.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
