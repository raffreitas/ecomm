using System.Text.Json.Serialization;

namespace Ecomm.Payments.Infrastructure.Payments.DTOs;

public record AsassCreatePaymentResponseDto
{
    [JsonPropertyName("id")] public string TransactionId { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}