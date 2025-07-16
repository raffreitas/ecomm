using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Shared.Abstractions;

public interface IEventSourceService
{
    Task SaveEvent<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : DomainEvent;
}
