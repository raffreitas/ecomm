using Ecomm.Payments.Domain.Primitives;

namespace Ecomm.Payments.Domain.Events;

public record PaymentRejectedEvent(Guid OrderId) : IDomainEvent;
