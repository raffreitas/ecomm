using Ecomm.Shared.Infrastructure.Observability.Correlation.Context;

namespace Ecomm.Shared.Infrastructure.Observability.Correlation.Factory;
public interface ICorrelationContextFactory
{
    public CorrelationContext Create(string correlationId);
    public CorrelationContext Create();
}
