using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Features.Products.CreateProduct;

namespace Ecomm.Catalog.Infrastructure.Messaging;

public sealed class EventResolver : IEventResolver
{
    public EventMetadata Resolve<TEvent>() where TEvent : IIntegrationEvent
    {
        if (typeof(TEvent) == typeof(ProductCreatedIntegrationEvent))
        {
            return new EventMetadata("product.created", "product.created", 1);
        }

        throw new InvalidOperationException($"Integration event '{typeof(TEvent).Name}' is not registered.");
    }
}
