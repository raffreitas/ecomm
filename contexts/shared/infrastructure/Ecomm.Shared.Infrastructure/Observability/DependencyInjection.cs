using Ecomm.Shared.Infrastructure.Messaging;
using Ecomm.Shared.Infrastructure.Observability.Correlation;
using Ecomm.Shared.Infrastructure.Observability.Correlation.Middleware;
using Ecomm.Shared.Infrastructure.Observability.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Npgsql;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ecomm.Shared.Infrastructure.Observability;

public static class DependencyInjection
{
    private const string OtelExporterOtplEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

    public static IServiceCollection AddObservabilityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOpenTelemetry(configuration);

        services.AddCorrelationConfiguration();

        return services;
    }

    public static WebApplication UseObservabilityConfiguration(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }

    private static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        const string serviceVersion = "1.0.0";
        string serviceName = AppDomain.CurrentDomain.FriendlyName;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion
            ))
            .WithTracing(builder => builder
                .AddSource(serviceName)
                .AddSource(MessagingDiagnostics.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(x => x.SetDbStatementForStoredProcedure = true)
            )
            .WithMetrics(builder => builder
                .AddMeter(serviceName)
                .AddMeter(MessagingDiagnostics.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddNpgsqlInstrumentation()
            )
            .WithLogging(null, options =>
            {
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.IncludeFormattedMessage = true;
            });

        var useOtlpExporter = !string.IsNullOrWhiteSpace(configuration[OtelExporterOtplEndpoint]);
        if (useOtlpExporter)
            services.AddOpenTelemetry().UseOtlpExporter();
    }
}
