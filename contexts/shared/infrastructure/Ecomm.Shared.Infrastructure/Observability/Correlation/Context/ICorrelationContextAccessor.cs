namespace Ecomm.Shared.Infrastructure.Observability.Correlation.Context;
public interface ICorrelationContextAccessor
{
    CorrelationContext? Context { get; set; }
}
