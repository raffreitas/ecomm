using Ecomm.Orders.Domain.Repositories;

using MediatR;

namespace Ecomm.Orders.Application.Orders.MarkOrderAsPaid;

public class MarkOrderAsPaidCommandHandler : IRequestHandler<MarkOrderAsPaidCommand>
{
    private readonly IOrderRepository _orderRepository;

    public MarkOrderAsPaidCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new Exception("Order not found");

        order.MarkAsPaid();
    }
}