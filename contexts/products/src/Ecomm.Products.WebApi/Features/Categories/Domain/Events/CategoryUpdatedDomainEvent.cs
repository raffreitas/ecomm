using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Categories.Domain.Events;

public sealed record CategoryUpdatedDomainEvent : DomainEvent
{
    public override Guid AggregateId { get; }
    public string Name { get; }
    public Guid? ParentCategoryId { get; }
    public CategoryUpdatedDomainEvent(Guid aggregateId, string name, Guid? parentCategoryId)
    {
        AggregateId = aggregateId;
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

    public static CategoryUpdatedDomainEvent FromCategory(Category category)
    {
        return new CategoryUpdatedDomainEvent(category.Id, category.Name, category.ParentCategoryId);
    }
}
