using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Shared.Domain.ValueObjects;

public sealed record Price : IValueObject
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    private Price(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Price amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, string currency) => new(amount, currency);

    public override string ToString() => $"{Amount} {Currency}";
}
