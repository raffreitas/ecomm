using Ecomm.Orders.Domain.Repositories;

using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsFailed;
internal class MarkOrderAsFailedCommandHandler : IRequestHandler<MarkOrderAsFailedCommand>
{
    private readonly IOrderRepository _orderRepository;

    public MarkOrderAsFailedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(MarkOrderAsFailedCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new Exception("Order not found");

        order.MarkAsFailed();
    }
}
