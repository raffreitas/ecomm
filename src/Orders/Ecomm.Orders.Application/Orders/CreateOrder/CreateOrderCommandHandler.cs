using Ecomm.Orders.Domain.DTOs;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using MediatR;

namespace Ecomm.Orders.Application.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var uniqueProductIds = request.Items.Select(x => x.ProductId).ToHashSet();

        var products = await _productRepository.GetProductsByIdsAsync(uniqueProductIds, cancellationToken);

        if (products.Count != uniqueProductIds.Count)
            throw new ApplicationException("Invalid number of products");

        var orderItemsDto = request.Items.Select(item =>
        {
            var product = products.First(x => x.Id == item.ProductId);
            return new CreateOrderItemDto(item.ProductId, item.Quantity, product.Price);
        });

        var createOrderDto = new CreateOrderDto(orderItemsDto);
        var order = Order.Create(createOrderDto);

        await _orderRepository.CreateAsync(order, cancellationToken);
    }
}