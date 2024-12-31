using FluentValidation;

namespace Ecomm.Orders.Application.Customers.Register;

public class RegisterCustomerCommandValidation : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}