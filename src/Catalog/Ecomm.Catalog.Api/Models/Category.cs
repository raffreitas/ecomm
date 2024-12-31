namespace Ecomm.Catalog.Api.Models;

public sealed class Category
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public IList<Product> Products { get; init; } = [];
}