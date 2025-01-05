namespace Ecomm.Catalog.Api.Models;

public sealed class Product
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public Category? Category { get; init; }
    public Guid CategoryId { get; set; }
}