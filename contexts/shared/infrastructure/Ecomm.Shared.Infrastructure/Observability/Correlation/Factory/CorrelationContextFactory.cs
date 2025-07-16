using Ecomm.Shared.Infrastructure.Observability.Correlation.Context;

namespace Ecomm.Shared.Infrastructure.Observability.Correlation.Factory;

internal sealed class CorrelationContextFactory(ICorrelationContextAccessor correlationContextAccessor) : ICorrelationContextFactory
{
    private readonly Lock _lock = new();

    public CorrelationContext Create(string correlationId)
    {
        lock (_lock)
        {
            var context = new CorrelationContext(correlationId);
            return correlationContextAccessor.Context = context;
        }
    }

    public CorrelationContext Create() => Create(Guid.NewGuid().ToString("N"));
}
