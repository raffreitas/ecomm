using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Infrastructure.Persistence.Repositories;

public class CustomerRepository(OrdersDbContext dbContext) : ICustomerRepository
{
    public async Task CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await dbContext.Customers.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers.FirstOrDefaultAsync(x => x.Id.Equals(customerId), cancellationToken);
    }
}