namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

public class OutboxEvent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    public OutboxEvent(string type, string content, DateTime occurredAt)
    {
        Type = type;
        Content = content;
        OccurredAt = occurredAt;
        RetryCount = 5;
    }

    public void MarkProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkError(string error)
    {
        RetryCount--;
        Error = error;
    }
}
