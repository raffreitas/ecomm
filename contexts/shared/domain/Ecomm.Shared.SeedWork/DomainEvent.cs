namespace Ecomm.Shared.SeedWork;

public abstract record DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public abstract Guid AggregateId { get; }
    public string Type => GetType().Name;
}