using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Features.Products.CreateProduct;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class EventResolver : IEventResolver
{
    public EventMetadata Resolve<TEvent>() where TEvent : IIntegrationEvent
    {
        if (typeof(TEvent) == typeof(ProductCreatedIntegrationEvent))
        {
            return new EventMetadata("catalog.product-created.v1", "catalog-events", 1);
        }

        throw new InvalidOperationException($"Integration event '{typeof(TEvent).Name}' is not registered.");
    }
}
