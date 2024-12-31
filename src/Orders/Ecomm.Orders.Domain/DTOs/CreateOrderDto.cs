namespace Ecomm.Orders.Domain.DTOs;

public record CreateOrderDto(IEnumerable<CreateOrderItemDto> Items);

public record CreateOrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);