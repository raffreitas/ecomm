using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Configurations;

internal sealed class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
    public void Configure(EntityTypeBuilder<InventoryEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.HasIndex(x => x.ProductId)
            .IsUnique();

        builder.OwnsOne(x => x.Quantity, quantity =>
        {
            quantity.Property(q => q.Value)
                .IsRequired()
                .HasColumnName("quantity");
        });
    }
}
