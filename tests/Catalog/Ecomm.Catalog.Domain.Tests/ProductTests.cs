using Ecomm.Catalog.Domain.Entities;

using Xunit;

namespace Ecomm.Catalog.Domain.Tests;

public sealed class ProductTests
{
    [Fact]
    public void Create_normalizes_values()
    {
        var product = Product.Create(" Keyboard ", " Mechanical ", 100m, "https://example.com/image.png", Guid.NewGuid());

        Assert.Equal("Keyboard", product.Name);
        Assert.Equal("Mechanical", product.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_rejects_non_positive_price(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Product.Create("Keyboard", "Mechanical", price, "https://example.com/image.png", Guid.NewGuid()));
    }

    [Fact]
    public void Create_rejects_invalid_image_url()
    {
        Assert.Throws<ArgumentException>(() =>
            Product.Create("Keyboard", "Mechanical", 100m, "image.png", Guid.NewGuid()));
    }
}
