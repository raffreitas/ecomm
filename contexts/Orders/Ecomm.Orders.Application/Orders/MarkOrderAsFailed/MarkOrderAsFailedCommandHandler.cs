using Ecomm.Orders.Application.Abstractions;

using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsFailed;

internal sealed class MarkOrderAsFailedCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<MarkOrderAsFailedCommand>
{
    public async Task Handle(MarkOrderAsFailedCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} was not found.");
        order.MarkAsFailed();
    }
}
