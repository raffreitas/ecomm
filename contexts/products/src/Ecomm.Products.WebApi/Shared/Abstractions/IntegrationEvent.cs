namespace Ecomm.Products.WebApi.Shared.Abstractions;

public abstract record IntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public abstract int Version { get; }
}
