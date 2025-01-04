using System.Text;
using System.Text.Json;

using Ecomm.Customers.Api.Messaging;
using Ecomm.Customers.Api.Models;
using Ecomm.Customers.Api.Repositories;
using Ecomm.Customers.Api.Requests;

using FluentValidation;

namespace Ecomm.Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly IValidator<CreateCustomerRequest> _validator;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMessageBusService messageBusService,
        IValidator<CreateCustomerRequest> validator)
    {
        _customerRepository = customerRepository;
        _messageBusService = messageBusService;
        _validator = validator;
    }

    public async Task CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var existsWithDocument = await _customerRepository.ExistsWithDocumentAsync(request.Document, cancellationToken);

        if (existsWithDocument)
            throw new Exception("This customer already exists");

        var customer = new Customer { Name = request.Name, Email = request.Email, Document = request.Document, };

        await _customerRepository.CreateAsync(customer, cancellationToken);

        var customerCreatedMessage = JsonSerializer.Serialize(customer);
        await _messageBusService.PublishAsync(
            queue: "customer.created",
            message: Encoding.UTF8.GetBytes(customerCreatedMessage),
            cancellationToken);
    }
}