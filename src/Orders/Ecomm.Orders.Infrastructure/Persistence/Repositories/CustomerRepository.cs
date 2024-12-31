using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

namespace Ecomm.Orders.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OrdersDbContext _dbContext;

    public CustomerRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}