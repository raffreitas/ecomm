namespace Ecomm.Products.WebApi.Shared.Domain.Abstractions;

public abstract record DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid AggregateId { get; init; }
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}