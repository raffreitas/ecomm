using System.Text.Json.Serialization;

namespace Ecomm.Payments.Infrastructure.Payments.DTOs;

public record AsassCreateCustomerResponseDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
};