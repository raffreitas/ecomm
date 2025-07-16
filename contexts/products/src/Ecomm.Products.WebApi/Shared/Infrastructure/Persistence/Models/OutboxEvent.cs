namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Models;

public class OutboxEvent(string type, string content, DateTime occurredAt, string correlationId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Type { get; private set; } = type;
    public string Content { get; private set; } = content;
    public DateTime OccurredAt { get; private set; } = occurredAt;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; } = 5;
    public string? Error { get; private set; }
    public string CorrelationId { get; private set; } = correlationId;

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
