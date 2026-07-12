using FluentValidation;

namespace Ecomm.Customers.Api.Requests;

public class CreateCustomerRequestValidation : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}