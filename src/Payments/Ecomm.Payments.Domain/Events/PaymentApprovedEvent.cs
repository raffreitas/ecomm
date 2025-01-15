using Ecomm.Payments.Domain.Primitives;

namespace Ecomm.Payments.Domain.Events;

public record PaymentApprovedEvent(Guid OrderId) : IDomainEvent;

