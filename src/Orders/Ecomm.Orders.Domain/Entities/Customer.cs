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
}