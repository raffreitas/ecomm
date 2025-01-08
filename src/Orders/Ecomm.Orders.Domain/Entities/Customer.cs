using System.Text.Json.Serialization;

using Ecomm.Orders.Domain.Primitives;

namespace Ecomm.Orders.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private init; }
    public string Document { get; private init; }

    // EF. Rel
    public IList<Order> Orders { get; private set; } = [];

    public Customer(string name, string document)
    {
        Name = name;
        Document = document;
    }

    [JsonConstructor]
    internal Customer(Guid id, string name, string document)
    {
        Id = id;
        Name = name;
        Document = document;
    }
}