namespace Ecomm.Catalog.Common.Messaging;

public interface IEventResolver
{
    EventMetadata Resolve<TEvent>() where TEvent : IIntegrationEvent;
}

public sealed record EventMetadata(string Type, string Destination, int Version);
