using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Application.Events;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Shared;

internal sealed class UnitOfWork(
    ApplicationDbContext dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEventsIfNeeded(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEventsIfNeeded(CancellationToken cancellationToken = default)
    {
        List<DomainEvent> domainEvents;
        do
        {
            domainEvents = GetAndClearDomainEventsFromAggregates();

            if (domainEvents.Count != 0)
                await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

        } while (domainEvents.Count != 0);
    }

    private List<DomainEvent> GetAndClearDomainEventsFromAggregates()
        => [.. dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .SelectMany(x =>
            {
                IEnumerable<DomainEvent> domainEvents = [.. x.DomainEvents];
                x.ClearDomainEvents();
                return domainEvents;
            })];
}
