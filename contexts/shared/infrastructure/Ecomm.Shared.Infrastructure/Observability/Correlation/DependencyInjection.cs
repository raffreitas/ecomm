using Ecomm.Shared.Infrastructure.Observability.Correlation.Context;
using Ecomm.Shared.Infrastructure.Observability.Correlation.Factory;
using Ecomm.Shared.Infrastructure.Observability.Correlation.HttpClient;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace Ecomm.Shared.Infrastructure.Observability.Correlation;

internal static class DependencyInjection
{
    public static IServiceCollection AddCorrelationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
        services.AddSingleton<ICorrelationContextFactory, CorrelationContextFactory>();

        services.TryAddTransient<CorrelationIdDelegateHandler>();

        services.ConfigureAll<HttpClientFactoryOptions>(options => options
            .HttpMessageHandlerBuilderActions
                .Add(builder => builder.AdditionalHandlers
                    .Add(builder.Services.GetRequiredService<CorrelationIdDelegateHandler>())));

        return services;
    }
}
