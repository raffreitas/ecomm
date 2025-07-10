using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Shared.Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
