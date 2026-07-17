using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;

using Xunit;

namespace Ecomm.Payments.Domain.Tests;

public sealed class PaymentTests
{
    [Fact]
    public void Approve_records_external_id()
    {
        var payment = CreatePayment();

        payment.MarkAsApproved("provider-123");

        Assert.Equal(PaymentStatus.Approved, payment.Status);
        Assert.Equal("provider-123", payment.ExternalPaymentId);
        Assert.Single(payment.DomainEvents);
    }

    [Fact]
    public void Reject_records_reason()
    {
        var payment = CreatePayment();

        payment.MarkAsRejected("insufficient funds", "provider-123");

        Assert.Equal(PaymentStatus.Rejected, payment.Status);
        Assert.Equal("insufficient funds", payment.RejectionReason);
        Assert.Equal("provider-123", payment.ExternalPaymentId);
    }

    [Fact]
    public void Completed_payment_cannot_transition_again()
    {
        var payment = CreatePayment();
        payment.MarkAsApproved("provider-123");

        Assert.Throws<InvalidOperationException>(() => payment.MarkAsRejected("declined"));
    }

    [Fact]
    public void Create_rejects_non_positive_total()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Payment(Guid.NewGuid(), 0, "123", "Ada"));
    }

    private static Payment CreatePayment() => new(Guid.NewGuid(), 10m, "123", "Ada");
}
