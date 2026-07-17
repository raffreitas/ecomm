namespace Ecomm.Messaging;

public sealed record EventEnvelope<TData>(
    Guid Id,
    string Type,
    int Version,
    DateTimeOffset OccurredAtUtc,
    string? CorrelationId,
    string? CausationId,
    TData Data);
