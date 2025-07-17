using Ecomm.Products.WebApi.Features.Categories.Domain.Events;
using Ecomm.Products.WebApi.Features.Categories.Events.Integration;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Categories.Events.Handlers;

public sealed class CategoryUpdatedDomainEventHandler(
    ILogger<CategoryUpdatedDomainEventHandler> logger,
    IEventOutboxService eventOutboxService
) : IDomainEventHandler<CategoryUpdatedDomainEvent>
{
    public async Task HandleAsync(CategoryUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Category] Category updated: CategoryId {CategoryId}, Name: {Name}, ParentCategoryId: {ParentCategoryId}", 
            domainEvent.AggregateId, domainEvent.Name, domainEvent.ParentCategoryId);

        var integrationEvent = new CategoryUpdatedIntegrationEvent(
            domainEvent.AggregateId, 
            domainEvent.Name, 
            domainEvent.ParentCategoryId);
        
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        
        logger.LogInformation("[Category] Integration event CategoryUpdatedIntegrationEvent published for Category {CategoryId}.", 
            domainEvent.AggregateId);
    }
}
