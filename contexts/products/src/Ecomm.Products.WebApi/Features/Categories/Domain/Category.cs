using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Categories.Domain;

public sealed class Category : AggregateRoot
{
    private readonly List<Category> _children = new();
    public string Name { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Category? Parent { get; private set; }
    public IReadOnlyList<Category> Children => _children.AsReadOnly();

    #region EF Constructor
#pragma warning disable CS8618
    private Category() { }
#pragma warning restore CS8618
    #endregion

    private Category(string name, Category? parent)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
        Name = name;
        Parent = parent;
        ParentCategoryId = parent?.Id;
        parent?._children.Add(this);
    }

    public static Category Create(string name, Category? parent = null)
        => new(name, parent);

    public void Update(string name, Category? parent = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
        Name = name;
        if (parent != null && parent.Id != Id)
        {
            Parent = parent;
            ParentCategoryId = parent.Id;
        }
    }
}
