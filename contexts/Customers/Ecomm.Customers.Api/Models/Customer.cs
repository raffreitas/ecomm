namespace Ecomm.Customers.Api.Models;

public sealed class Customer
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Document Document { get; private set; } = null!;

    private Customer() { }

    public static Customer Create(string name, string email, string document)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.", nameof(name));

        return new Customer
        {
            Name = name.Trim(),
            Email = Email.Create(email),
            Document = Document.Create(document),
        };
    }
}

public sealed record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (!System.Net.Mail.MailAddress.TryCreate(value, out var address))
            throw new ArgumentException("A valid email is required.", nameof(value));
        return new Email(address.Address.ToLowerInvariant());
    }

    public override string ToString() => Value;
}

public sealed record Document
{
    public string Value { get; }
    private Document(string value) => Value = value;

    public static Document Create(string value)
    {
        var normalized = new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray());
        if (normalized.Length is < 5 or > 20)
            throw new ArgumentException("Document must contain between 5 and 20 letters or digits.", nameof(value));
        return new Document(normalized);
    }

    public override string ToString() => Value;
}
