using Ecomm.Orders.Application.Abstractions;

using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsPaid;

public sealed class MarkOrderAsPaidCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<MarkOrderAsPaidCommand>
{
    public async Task Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} was not found.");
        order.MarkAsPaid();
    }
}
