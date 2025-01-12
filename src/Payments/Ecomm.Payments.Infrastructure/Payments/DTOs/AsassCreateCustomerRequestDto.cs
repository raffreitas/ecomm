using System.Text.Json.Serialization;

namespace Ecomm.Payments.Infrastructure.Payments.DTOs;

public record AsassCreateCustomerRequestDto
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("cpfCnpj")] public string Document { get; set; } = string.Empty;
};