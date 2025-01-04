using Ecomm.Customers.Api.Models;
using Ecomm.Customers.Api.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Persistence.Repositories;

public class CustomerRepository(CustomersDbContext dbContext) : ICustomerRepository
{
    public async Task CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await dbContext.Customers.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers.AnyAsync(x => x.Document == document, cancellationToken);
    }
}