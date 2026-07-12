using Ecomm.Orders.Domain.Entities;

namespace Ecomm.Orders.Domain.DTOs;

public record CreateOrderDto(Customer Customer, string CardHash, IEnumerable<CreateOrderItemDto> Items);

public record CreateOrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);