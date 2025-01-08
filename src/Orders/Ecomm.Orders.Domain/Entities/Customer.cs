using System.Text.Json.Serialization;

using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private init; }
    public string Email { get; private init; }

    // EF. Rel
    public IList<Order> Orders { get; private set; } = [];

    public Customer(string name, string email)
    {
        Name = name;
        Email = email;
    }

    [JsonConstructor]
    internal Customer(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}