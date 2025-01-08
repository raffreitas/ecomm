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

    [JsonConstructor]
    internal Product(Guid id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}