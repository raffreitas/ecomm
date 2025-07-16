using Ecomm.Shared.SeedWork;

namespace Ecomm.Shared.Infrastructure.EventSourcing.Abstractions;

public interface IEventStoreService
{
    Task SaveEvent<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : DomainEvent;
}