using Ecomm.Payments.Domain.Enums;
using Ecomm.Payments.Domain.Primitives;

namespace Ecomm.Payments.Domain.Entities;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public decimal Total { get; private set; }
    public PaymentStatus Status { get; private set; }

    public Payment(Guid orderId, decimal total)
    {
        OrderId = orderId;
        Total = total;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsApproved()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark as paid because status is {Status}.");
        Status = PaymentStatus.Approved;
    }

    public void MarkAsRejected()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark as paid because status is {Status}.");

        Status = PaymentStatus.Rejected;
    }
}