using FluentValidation;

namespace Ecomm.Orders.Application.Orders.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CardHash).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(x =>
            {
                x.RuleFor(item => item.Quantity).NotEmpty().GreaterThan(0);
                x.RuleFor(item => item.ProductId).NotEmpty().NotEqual(Guid.Empty);
            });
    }
}