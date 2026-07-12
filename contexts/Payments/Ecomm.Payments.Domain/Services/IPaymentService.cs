using Ecomm.Payments.Domain.DTOs;
using Ecomm.Payments.Domain.Entities;

namespace Ecomm.Payments.Domain.Services;

public interface IPaymentService
{
    Task<ProcessPaymentResponseDto> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}