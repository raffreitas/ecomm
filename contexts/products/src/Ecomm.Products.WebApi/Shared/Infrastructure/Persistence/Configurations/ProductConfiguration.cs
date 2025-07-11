using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsMany(x => x.Images, images =>
        {
            images.ToTable("product_images");

            images.WithOwner().HasForeignKey("product_id");

            images.Property(x => x.Url).IsRequired().HasMaxLength(200);
            images.Property(x => x.AltText).IsRequired().HasMaxLength(100);
        });

        builder
            .HasMany<ProductCategory>()
            .WithOne()
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsListed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(x => x.Price, price =>
        {
            price.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });
    }
}
