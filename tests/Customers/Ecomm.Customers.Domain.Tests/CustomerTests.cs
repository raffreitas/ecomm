using Ecomm.Customers.Api.Models;

using Xunit;

namespace Ecomm.Customers.Domain.Tests;

public sealed class CustomerTests
{
    [Fact]
    public void Create_normalizes_email_and_document()
    {
        var customer = Customer.Create(" Ada ", "ADA@EXAMPLE.COM", "123.456");

        Assert.Equal("Ada", customer.Name);
        Assert.Equal("ada@example.com", customer.Email.Value);
        Assert.Equal("123456", customer.Document.Value);
    }

    [Fact]
    public void Create_rejects_invalid_email()
    {
        Assert.Throws<ArgumentException>(() => Customer.Create("Ada", "invalid", "123456"));
    }

    [Fact]
    public void Create_rejects_short_document()
    {
        Assert.Throws<ArgumentException>(() => Customer.Create("Ada", "ada@example.com", "12"));
    }
}
