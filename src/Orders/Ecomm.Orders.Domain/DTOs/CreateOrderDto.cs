namespace Ecomm.Orders.Domain.DTOs;

public record CreateOrderDto(Guid CustomerId, string CardHash, IEnumerable<CreateOrderItemDto> Items);

public record CreateOrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);