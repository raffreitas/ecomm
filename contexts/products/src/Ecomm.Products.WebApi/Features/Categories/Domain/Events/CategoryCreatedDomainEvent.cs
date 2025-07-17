using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Categories.Domain.Events;

public sealed record CategoryCreatedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public string Name { get; }
    public Guid? ParentCategoryId { get; }

    public CategoryCreatedDomainEvent(Guid aggregateId, string name, Guid? parentCategoryId)
    {
        AggregateId = aggregateId;
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

    public static CategoryCreatedDomainEvent FromCategory(Category category)
    {
        return new CategoryCreatedDomainEvent(category.Id, category.Name, category.ParentCategoryId);
    }
}
