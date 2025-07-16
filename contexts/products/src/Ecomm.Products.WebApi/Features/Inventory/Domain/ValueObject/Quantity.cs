using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;

public sealed record Quantity : IValueObject
{
    public int Value { get; init; }
    private Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(value));
        Value = value;
    }
    public static Quantity Create(int value) => new(value);
    public override string ToString() => Value.ToString();

    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static explicit operator Quantity(int value) => Create(value);
}
