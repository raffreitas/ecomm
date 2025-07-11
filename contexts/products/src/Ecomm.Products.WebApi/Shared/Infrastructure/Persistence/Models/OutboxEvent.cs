namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

public sealed record OutboxEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Type { get; }
    public string Content { get; }
    public DateTime OccurredAt { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; }
    public int RetryCount { get; }
    public string? Error { get; }

    public OutboxEvent(string type, string content, DateTime occurredAt)
    {
        Type = type;
        Content = content;
        OccurredAt = occurredAt;
        RetryCount = 5;
    }
}
