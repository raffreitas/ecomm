namespace Ecomm.Orders.Domain.Primitives;

public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    protected Entity() { }
}