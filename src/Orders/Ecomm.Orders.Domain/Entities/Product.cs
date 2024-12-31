using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public sealed class Product : Entity
{
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string ImageUrl { get; init; }

    // EF. Rel.
    public IList<OrderItem> OrderItems { get; private set; } = [];

    public Product(string name, string description, decimal price, string imageUrl)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
    }
}