namespace Ecomm.Catalog.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public Category? Category { get; private set; }
    public Guid CategoryId { get; private set; }

    private Product() { }

    public static Product Create(
        string name,
        string description,
        decimal price,
        string imageUrl,
        Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));
        if (name.Trim().Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description is required.", nameof(description));
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be positive.");
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Image URL must be an absolute HTTP or HTTPS URL.", nameof(imageUrl));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category id is required.", nameof(categoryId));

        return new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            ImageUrl = uri.AbsoluteUri,
            CategoryId = categoryId,
        };
    }
}
