using Ecomm.Orders.Domain.DTOs;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace Ecomm.Orders.Application.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateOrderCommand> _validator;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IValidator<CreateOrderCommand> validator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        // TODO: Inject service to get the customerId based on the user on request
        var customerId = Guid.Empty;

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var uniqueProductIds = request.Items.Select(x => x.ProductId).ToHashSet();

        var products = await _productRepository.GetProductsByIdsAsync(uniqueProductIds, cancellationToken);

        if (products.Count != uniqueProductIds.Count)
            throw new ApplicationException("Invalid number of products");

        var orderItemsDto = request.Items.Select(item =>
        {
            var product = products.First(x => x.Id == item.ProductId);
            return new CreateOrderItemDto(item.ProductId, item.Quantity, product.Price);
        });

        var createOrderDto = new CreateOrderDto(customerId, orderItemsDto);
        var order = Order.Create(createOrderDto);

        await _orderRepository.CreateAsync(order, cancellationToken);
    }
}