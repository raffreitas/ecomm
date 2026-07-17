namespace Ecomm.Catalog.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public IList<Product> Products { get; private set; } = [];

    private Category() { }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));
        if (name.Trim().Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));
        return new Category { Name = name.Trim() };
    }
}
