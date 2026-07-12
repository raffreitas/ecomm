namespace Ecomm.Payments.Domain.Primitives;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public ICollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity() { }

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}