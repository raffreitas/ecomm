using Ecomm.Orders.Domain.Entities;

namespace Ecomm.Orders.Domain.Repositories;

public interface ICustomerRepository
{
    Task CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}