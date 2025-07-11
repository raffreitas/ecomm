using Ecomm.Products.WebApi.Shared.Domain.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.ValueObjects;

namespace Ecomm.Products.WebApi.Features.Products.Domain.Events;

public sealed record ProductCreatedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public Guid ProductId { get; }
    public string Name { get; }
    public string? Description { get; }
    public Price Price { get; }
    public Guid[] Categories { get; }

    public ProductCreatedDomainEvent(
        Guid productId,
        string name,
        string? description,
        Price price,
        Guid[] categories
    )
    {
        AggregateId = productId;
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        Categories = categories;
    }

    public static ProductCreatedDomainEvent FromAggregate(Product product)
        => new(
            productId: product.Id,
            name: product.Name,
            description: product.Description,
            price: product.Price,
            categories: [.. product.CategoryIds]
        );
}
