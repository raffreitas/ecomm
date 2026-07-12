using Ecomm.Customers.Api.Requests;

namespace Ecomm.Customers.Api.Services;

public interface ICustomerService
{
    Task CreateAsync(CreateCustomerRequest customerRequest, CancellationToken cancellationToken = default);
}