using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;

namespace Ecomm.Payments.Application.Abstractions;

public interface IPaymentRepository
{
    Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task CreateAsync(Payment payment, CancellationToken cancellationToken = default);
}

public interface IPaymentService
{
    Task<ProcessPaymentResponse> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}

public sealed record ProcessPaymentResponse(string TransactionId, PaymentStatus Status);
