namespace Ecomm.Payments.Domain.Primitives;

public abstract class Entity
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    protected Entity() { }
}