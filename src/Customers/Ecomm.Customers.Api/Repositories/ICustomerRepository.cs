using Ecomm.Customers.Api.Models;

namespace Ecomm.Customers.Api.Repositories;

public interface ICustomerRepository
{
    Task CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithDocumentAsync(string document, CancellationToken cancellationToken = default);
}