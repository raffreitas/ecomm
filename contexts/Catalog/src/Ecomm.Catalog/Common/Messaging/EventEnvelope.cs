namespace Ecomm.Catalog.Common.Messaging;

public sealed record EventEnvelope<TEvent>(
    Guid Id,
    string Type,
    int Version,
    DateTimeOffset OccurredAtUtc,
    TEvent Data);
