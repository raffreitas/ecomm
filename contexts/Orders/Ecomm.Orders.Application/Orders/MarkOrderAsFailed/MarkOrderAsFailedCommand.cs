using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsFailed;
public record MarkOrderAsFailedCommand(Guid OrderId) : IRequest;
