namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

public sealed record ProductCategory(Guid ProductId, Guid CategoryId)
{
    public Guid ProductId { get; init; } = ProductId;
    public Guid CategoryId { get; init; } = CategoryId;
}
