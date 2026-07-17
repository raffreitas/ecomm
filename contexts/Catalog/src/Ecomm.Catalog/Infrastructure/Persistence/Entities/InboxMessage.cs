namespace Ecomm.Catalog.Infrastructure.Persistence.Entities;

public sealed class InboxMessage
{
    public Guid MessageId { get; init; }
    public string ConsumerName { get; init; } = string.Empty;
    public DateTimeOffset ProcessedAtUtc { get; init; }
}
