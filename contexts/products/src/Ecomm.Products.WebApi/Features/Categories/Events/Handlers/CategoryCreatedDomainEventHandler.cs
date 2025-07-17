using Ecomm.Products.WebApi.Features.Categories.Domain.Events;
using Ecomm.Products.WebApi.Features.Categories.Events.Integration;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Shared.SeedWork;

namespace Ecomm.Products.WebApi.Features.Categories.Events.Handlers;

public sealed class CategoryCreatedDomainEventHandler(
    ILogger<CategoryCreatedDomainEventHandler> logger,
    IEventOutboxService eventOutboxService
) : IDomainEventHandler<CategoryCreatedDomainEvent>
{
    public async Task HandleAsync(CategoryCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Category] New category created: CategoryId {CategoryId}, Name: {Name}, ParentCategoryId: {ParentCategoryId}", 
            domainEvent.AggregateId, domainEvent.Name, domainEvent.ParentCategoryId);

        var integrationEvent = new CategoryCreatedIntegrationEvent(
            domainEvent.AggregateId, 
            domainEvent.Name, 
            domainEvent.ParentCategoryId);
        
        await eventOutboxService.AddAsync(integrationEvent, cancellationToken);
        
        logger.LogInformation("[Category] Integration event CategoryCreatedIntegrationEvent published for Category {CategoryId}.", 
            domainEvent.AggregateId);
    }
}
