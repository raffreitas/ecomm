namespace Ecomm.Payments.Application.IntegrationEvents;

public sealed record PaymentApprovedIntegrationEvent(Guid OrderId, Guid PaymentId, string ExternalPaymentId);

public sealed record PaymentRejectedIntegrationEvent(
    Guid OrderId,
    Guid PaymentId,
    string? ExternalPaymentId,
    string Reason);
