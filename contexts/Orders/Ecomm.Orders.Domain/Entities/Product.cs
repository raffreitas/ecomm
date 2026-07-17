using System.Text.Json.Serialization;

using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class Product : Entity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    // EF. Rel.
    public IList<OrderItem> OrderItems { get; private set; } = [];

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public static Product CreateSnapshot(Guid id, string name, decimal price)
    {
        if (id == Guid.Empty) throw new ArgumentException("Product id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
        return new Product(id, name.Trim(), price);
    }

    [JsonConstructor]
    internal Product(Guid id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}
