using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Payments.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.Total).IsRequired();
        builder.Property(x => x.Status).IsRequired()
            .HasConversion(v => v.ToString(), v => Enum.Parse<PaymentStatus>(v));

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}