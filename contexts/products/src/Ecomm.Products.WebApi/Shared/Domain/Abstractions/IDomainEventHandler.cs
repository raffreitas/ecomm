namespace Ecomm.Products.WebApi.Shared.Domain.Abstractions;

public interface IDomainEventHandler<in T> where T : DomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
}
