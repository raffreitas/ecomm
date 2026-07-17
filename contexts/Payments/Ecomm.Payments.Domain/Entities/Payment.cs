using Ecomm.Payments.Domain.Enums;
using Ecomm.Payments.Domain.Events;
using Ecomm.Payments.Domain.Primitives;

namespace Ecomm.Payments.Domain.Entities;

public sealed class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public decimal Total { get; private set; }
    public string CustomerDocument { get; private set; }
    public string CustomerName { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? ExternalPaymentId { get; private set; }
    public string? RejectionReason { get; private set; }

    public Payment(Guid orderId, decimal total, string customerDocument, string customerName)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order id is required.", nameof(orderId));
        if (total <= 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Total must be positive.");
        if (string.IsNullOrWhiteSpace(customerDocument))
            throw new ArgumentException("Customer document is required.", nameof(customerDocument));
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));

        OrderId = orderId;
        Total = total;
        CustomerDocument = customerDocument.Trim();
        CustomerName = customerName.Trim();
        Status = PaymentStatus.Pending;
    }

    public void MarkAsApproved(string externalPaymentId)
    {
        EnsurePending();
        if (string.IsNullOrWhiteSpace(externalPaymentId))
            throw new ArgumentException("External payment id is required.", nameof(externalPaymentId));

        ExternalPaymentId = externalPaymentId;
        Status = PaymentStatus.Approved;
        AddDomainEvent(new PaymentApprovedEvent(OrderId));
    }

    public void MarkAsRejected(string rejectionReason, string? externalPaymentId = null)
    {
        EnsurePending();
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required.", nameof(rejectionReason));

        ExternalPaymentId = externalPaymentId;
        RejectionReason = rejectionReason.Trim();
        Status = PaymentStatus.Rejected;
        AddDomainEvent(new PaymentRejectedEvent(OrderId));
    }

    private void EnsurePending()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Payment {Id} cannot transition from {Status}.");
    }
}
