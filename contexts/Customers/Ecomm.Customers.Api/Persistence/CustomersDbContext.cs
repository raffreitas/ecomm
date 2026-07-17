using Ecomm.Customers.Api.Models;
using Ecomm.Messaging;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Persistence;

public class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        modelBuilder.AddMessagingEntities();
        base.OnModelCreating(modelBuilder);
    }
}
