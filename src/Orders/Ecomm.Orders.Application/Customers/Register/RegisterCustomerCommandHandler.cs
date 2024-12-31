using Ecomm.Orders.Application.Identity.DTOs;
using Ecomm.Orders.Application.Identity.Services;
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace Ecomm.Orders.Application.Customers.Register;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<RegisterCustomerCommand> _validator;

    public RegisterCustomerCommandHandler(
        IIdentityService identityService,
        ICustomerRepository customerRepository,
        IValidator<RegisterCustomerCommand> validator)
    {
        _identityService = identityService;
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var registerUserDto = new RegisterUserDto(request.Name, request.Email, request.Password);
        var identityResponse = await _identityService.RegisterAsync(registerUserDto);

        var customer = new Customer(request.Name, request.Email, identityResponse.UserId);

        await _customerRepository.CreateAsync(customer, cancellationToken);
    }
}