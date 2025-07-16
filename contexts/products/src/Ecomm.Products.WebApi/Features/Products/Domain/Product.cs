using Ecomm.Products.WebApi.Features.Products.Domain.Events;
using Ecomm.Products.WebApi.Features.Products.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Domain.ValueObjects;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Products.Domain;

public sealed class Product : AggregateRoot
{
    private readonly List<Guid> _categoryIds = [];
    private readonly List<Image> _images = [];

    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsListed { get; private set; }
    public Price Price { get; private set; }
    public IReadOnlyList<Image> Images => _images.AsReadOnly();
    public IReadOnlyList<Guid> CategoryIds => _categoryIds.AsReadOnly();

    #region EF Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Product()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    #endregion

    private Product(string name, string description, Price price, IEnumerable<Guid> categoryIds)
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
        _categoryIds = categoryIds?.ToList() ?? [];
        IsListed = false;

        AddDomainEvent(ProductCreatedDomainEvent.FromAggregate(this));
    }

    public static Product Create(string name, string description, Price price, IEnumerable<Guid> categoryIds)
        => new(name, description, price, categoryIds);

    public void AddImage(Image image)
        => _images.Add(image);

    public void AddCategory(Guid categoryId)
        => _categoryIds.Add(categoryId);

    public void Update(string name, string description, Price price, IEnumerable<Guid> categoryIds)
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
        _categoryIds.Clear();
        if (categoryIds is not null)
            _categoryIds.AddRange(categoryIds);
    }

    public void ToggleListing()
        => IsListed = !IsListed;
}
