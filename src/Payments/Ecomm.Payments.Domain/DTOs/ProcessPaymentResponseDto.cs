using Ecomm.Payments.Domain.Enums;

namespace Ecomm.Payments.Domain.DTOs;

public record ProcessPaymentResponseDto(string TransactionId, PaymentStatus Status);