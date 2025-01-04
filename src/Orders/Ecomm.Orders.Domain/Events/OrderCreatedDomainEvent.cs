using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Events;

public record OrderCreatedDomainEvent(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    string CardHash,
    decimal Total) : IDomainEvent;