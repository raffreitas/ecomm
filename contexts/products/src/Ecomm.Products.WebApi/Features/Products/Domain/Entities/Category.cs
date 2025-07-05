using Ecomm.Products.WebApi.Shared.Domain.Abstractions;

namespace Ecomm.Products.WebApi.Features.Products.Domain.Entities;

public sealed class Category : Entity
{
    public string Name { get; init; }

    private Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

        Name = name;
    }

    public static Category Create(string name) => new(name);
}
