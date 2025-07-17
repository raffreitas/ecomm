using Ecomm.Products.WebApi.Shared.Abstractions;

namespace Ecomm.Products.WebApi.Features.Categories.Events.Integration;

public sealed record CategoryCreatedIntegrationEvent : IntegrationEvent
{
    public override int Version => 1;
    public Guid CategoryId { get; init; }
    public string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }

    public CategoryCreatedIntegrationEvent(Guid categoryId, string name, Guid? parentCategoryId)
    {
        CategoryId = categoryId;
        Name = name;
        ParentCategoryId = parentCategoryId;
    }
}
