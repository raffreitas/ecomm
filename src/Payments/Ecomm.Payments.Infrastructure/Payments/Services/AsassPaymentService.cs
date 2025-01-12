using System.Net.Http.Json;
using System.Text.Json;

using Ecomm.Payments.Domain.DTOs;
using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;
using Ecomm.Payments.Domain.Services;
using Ecomm.Payments.Infrastructure.Payments.DTOs;
using Ecomm.Payments.Infrastructure.Payments.Settings;

using Microsoft.Extensions.Options;

namespace Ecomm.Payments.Infrastructure.Payments.Services;

public class AsassPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;

    public AsassPaymentService(IHttpClientFactory httpClientFactory, IOptions<PaymentSettings> options)
    {
        var paymentSettings = options.Value;

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(paymentSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("access_token", paymentSettings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Ecomm.Payments");
    }

    public async Task<ProcessPaymentResponseDto> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        var createCustomerDto = new AsassCreateCustomerRequestDto
        {
            Name = payment.CustomerName, Document = payment.CustomerDocument
        };
        var asassCustomer = await CreateCustomerAsync(createCustomerDto, cancellationToken);

        var createPaymentRequest = new AsassCreatePaymentRequestDto
        {
            Amount = payment.Total,
            CustomerId = asassCustomer.Id,
            DueDate = DateTime.Now.ToString("yyyy-MM-dd"),
            PaymentMethod = "CREDIT_CARD"
        };
        var createPaymentResponse = await CreatePaymentAsync(createPaymentRequest, cancellationToken);

        var paymentStatus = createPaymentResponse.Status switch
        {
            "CONFIRMED" => PaymentStatus.Approved,
            "PENDING" => PaymentStatus.Pending,
            _ => PaymentStatus.Rejected
        };
        return new ProcessPaymentResponseDto(createPaymentResponse.TransactionId, paymentStatus);
    }

    private async Task<AsassCreatePaymentResponseDto> CreatePaymentAsync(
        AsassCreatePaymentRequestDto dto,
        CancellationToken cancellationToken)
    {
        // TODO: Send the card hash on request and solve the error on the document of a provider/client

        var response = await _httpClient.PostAsJsonAsync("api/v3/payments", dto, cancellationToken);
        // response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStringAsync(cancellationToken);
        var createPaymentResponse = JsonSerializer.Deserialize<AsassCreatePaymentResponseDto>(stream);

        if (createPaymentResponse is null)
            throw new Exception("Unable to process payment at this time");

        return createPaymentResponse;
    }

    private async Task<AsassCreateCustomerResponseDto> CreateCustomerAsync(
        AsassCreateCustomerRequestDto dto,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v3/customers", dto, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStringAsync(cancellationToken);

        var createCustomerResponseDto = JsonSerializer.Deserialize<AsassCreateCustomerResponseDto>(stream);

        if (createCustomerResponseDto is null)
            throw new Exception("Unable to process payment at this time");

        return createCustomerResponseDto;
    }
}