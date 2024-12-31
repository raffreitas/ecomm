using Ecomm.Catalog.Api.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Catalog.Api.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.HasMany<Product>(c => c.Products)
            .WithOne(p => p.Category);
    }
}