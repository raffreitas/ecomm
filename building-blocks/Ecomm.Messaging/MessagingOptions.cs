namespace Ecomm.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "Messaging:ServiceBus";
    public string? ConnectionString { get; set; }
    public string? FullyQualifiedNamespace { get; set; }
}

public sealed class OutboxOptions
{
    public const string SectionName = "Messaging:Outbox";
    public int BatchSize { get; set; } = 20;
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan InitialRetryDelay { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan MaximumRetryDelay { get; set; } = TimeSpan.FromMinutes(5);
}
