namespace Ecomm.Orders.Domain.Primitives;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected init; } = Guid.NewGuid();
    public ICollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity() { }

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}