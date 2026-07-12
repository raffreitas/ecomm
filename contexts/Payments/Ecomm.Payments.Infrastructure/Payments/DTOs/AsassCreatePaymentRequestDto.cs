using System.Text.Json.Serialization;

namespace Ecomm.Payments.Infrastructure.Payments.DTOs;

public record AsassCreatePaymentRequestDto
{
    [JsonPropertyName("customer")] public string CustomerId { get; set; } = string.Empty;
    [JsonPropertyName("billingType")] public string PaymentMethod { get; set; } = string.Empty;
    [JsonPropertyName("value")] public decimal Amount { get; set; }
    [JsonPropertyName("dueDate")] public string DueDate { get; set; } = string.Empty;
}