using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsPaid;

public record MarkOrderAsPaidCommand(Guid OrderId) : IRequest;