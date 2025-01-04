using Ecomm.Payments.Domain.DTOs;
using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Services;

namespace Ecomm.Payments.Infrastructure.Payments;

public class PaymentService : IPaymentService
{
    public Task<ProcessPaymentResponseDto> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}