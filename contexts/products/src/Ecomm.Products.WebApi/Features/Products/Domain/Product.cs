using Ecomm.Products.WebApi.Features.Products.Domain.Entities;
using Ecomm.Products.WebApi.Features.Products.Domain.Events;
using Ecomm.Products.WebApi.Features.Products.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Domain.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.ValueObjects;

namespace Ecomm.Products.WebApi.Features.Products.Domain;

public sealed class Product : AggregateRoot
{
    private readonly List<Category> _categories = [];
    private readonly List<Image> _images = [];

    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsListed { get; private set; }
    public Price Price { get; private set; }
    public IReadOnlyList<Image> Images => _images.AsReadOnly();
    public IReadOnlyList<Category> Categories => _categories.AsReadOnly();

    #region EF Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Product()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    #endregion

    private Product(string name, string description, Price price, Category[] categories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty.", nameof(description));
        if (price is null)
            throw new ArgumentNullException(nameof(price), "Price cannot be null.");

        Name = name;
        Description = description;
        Price = price;
        _categories = [.. categories];
        IsListed = false;
    }

    public static Product Create(string name, string description, Price price, Category[] categories)
    {
        var product = new Product(name, description, price, categories);

        product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id, name, description, price));

        return product;
    }

    public void AddImage(Image image)
    {
        _images.Add(image);
    }

    public void Update(string name, string description, Price price, Category[] categories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty.", nameof(description));
        if (price is null)
            throw new ArgumentNullException(nameof(price), "Price cannot be null.");

        Name = name;
        Description = description;
        Price = price;
        _categories.Clear();
        _categories.AddRange(categories);
    }

    public void ToggleListing()
    {
        IsListed = !IsListed;
    }
}
