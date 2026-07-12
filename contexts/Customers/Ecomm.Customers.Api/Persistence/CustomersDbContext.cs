using Ecomm.Customers.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Persistence;

public class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}