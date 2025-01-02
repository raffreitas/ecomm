namespace Ecomm.Orders.Domain.Primitives;

public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public ICollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity() { }
    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}