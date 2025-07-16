using System.Collections.Concurrent;

using Ecomm.Shared.Infrastructure.EventSourcing.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Shared.Application.Events;

internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (DomainEvent domainEvent in domainEvents)
        {
            Type domainEventType = domainEvent.GetType();

            Type handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(IDomainEventHandler<>).MakeGenericType(et));

            var eventSourceService = serviceProvider.GetRequiredService<IEventStoreService>();
            await eventSourceService.SaveEvent(domainEvent, cancellationToken);

            IEnumerable<object?> handlers = serviceProvider.GetServices(handlerType);

            foreach (object? handler in handlers)
            {
                if (handler is null) continue;

                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);

                await handlerWrapper.Handle(domainEvent, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task Handle(DomainEvent domainEvent, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            Type wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));

            return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }
    }

    private sealed class HandlerWrapper<T>(object handler) : HandlerWrapper where T : DomainEvent
    {
        private readonly IDomainEventHandler<T> _handler = (IDomainEventHandler<T>)handler;

        public override async Task Handle(
            DomainEvent domainEvent,
            CancellationToken cancellationToken)
        {
            await _handler.HandleAsync((T)domainEvent, cancellationToken);
        }
    }
}
