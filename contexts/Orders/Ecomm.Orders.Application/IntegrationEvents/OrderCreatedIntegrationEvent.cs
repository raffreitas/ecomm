namespace Ecomm.Orders.Application.IntegrationEvents;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string CustomerName,
    string CustomerDocument,
    string CardHash,
    decimal Total);
