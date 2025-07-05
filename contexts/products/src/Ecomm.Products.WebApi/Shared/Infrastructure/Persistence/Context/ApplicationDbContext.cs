using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryEntity> Inventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<DomainEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
