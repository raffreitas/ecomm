using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Orders.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Total).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion(
            v => v.ToString(),
            v => Enum.Parse<OrderStatus>(v));
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasMany<OrderItem>()
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId);

        builder.HasOne<Customer>(x => x.Customer)
            .WithMany(x => x.Orders);
    }
}