using Ecomm.Catalog.Api.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Catalog.Api.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(100);

        builder.HasOne<Category>(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);
    }
}