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

    public static Customer CreateSnapshot(Guid id, string name, string document)
    {
        if (id == Guid.Empty) throw new ArgumentException("Customer id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Customer name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(document)) throw new ArgumentException("Customer document is required.", nameof(document));
        return new Customer(id, name.Trim(), document.Trim());
    }

    [JsonConstructor]
    internal Customer(Guid id, string name, string document)
    {
        Id = id;
        Name = name;
        Document = document;
    }
}
