namespace Ecomm.Messaging;

public sealed record OutboxDispatchMessage(
    Guid Id,
    string Type,
    int Version,
    string Destination,
    string Payload,
    string? CorrelationId,
    string? CausationId,
    int Attempts);

public sealed class InboxMessage
{
    public Guid MessageId { get; init; }
    public string ConsumerName { get; init; } = string.Empty;
    public DateTimeOffset ProcessedAtUtc { get; init; }
}

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public int Version { get; init; }
    public string Destination { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
    public string? CausationId { get; init; }
    public int Attempts { get; set; }
    public DateTimeOffset NextAttemptAtUtc { get; set; }
    public Guid? LeaseId { get; set; }
    public DateTimeOffset? LeaseExpiresAtUtc { get; set; }
    public DateTimeOffset? ProcessedAtUtc { get; set; }
    public string? LastError { get; set; }
}
