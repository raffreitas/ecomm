using System.ComponentModel.DataAnnotations;

namespace Ecomm.Payments.Infrastructure.Payments.Settings;

public class PaymentSettings
{
    [Required] public string BaseUrl { get; set; } = string.Empty;
    [Required] public string ApiKey { get; set; } = string.Empty;
}