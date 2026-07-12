using CategoryRequest = Ecomm.Catalog.Features.Categories.CreateCategory.Request;
using CategoryValidator = Ecomm.Catalog.Features.Categories.CreateCategory.Validator;
using ProductRequest = Ecomm.Catalog.Features.Products.CreateProduct.Request;
using ProductValidator = Ecomm.Catalog.Features.Products.CreateProduct.Validator;

namespace Ecomm.Catalog.UnitTests;

public sealed class ValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Category_name_must_contain_text(string name)
    {
        var result = await new CategoryValidator().ValidateAsync(new CategoryRequest(name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CategoryRequest.Name));
    }

    [Fact]
    public async Task Category_name_must_respect_database_limit()
    {
        var result = await new CategoryValidator().ValidateAsync(new CategoryRequest(new string('a', 101)));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Valid_product_request_is_accepted()
    {
        var request = ValidProductRequest();

        var result = await new ProductValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Description", 10, "https://example.com/image.png")]
    [InlineData("Product", " ", 10, "https://example.com/image.png")]
    [InlineData("Product", "Description", 0, "https://example.com/image.png")]
    [InlineData("Product", "Description", -1, "https://example.com/image.png")]
    [InlineData("Product", "Description", 10, "image.png")]
    [InlineData("Product", "Description", 10, "ftp://example.com/image.png")]
    public async Task Invalid_product_values_are_rejected(
        string name,
        string description,
        decimal price,
        string imageUrl)
    {
        var request = new ProductRequest(name, description, price, imageUrl, Guid.NewGuid());

        var result = await new ProductValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Product_category_id_must_not_be_empty()
    {
        var request = ValidProductRequest() with { CategoryId = Guid.Empty };

        var result = await new ProductValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    private static ProductRequest ValidProductRequest()
    {
        return new ProductRequest(
            "Product",
            "Description",
            10,
            "https://example.com/image.png",
            Guid.NewGuid());
    }
}
