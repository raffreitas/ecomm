using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ecomm.ServiceDefaults;

public static class ServiceDefaultsExtensions
{
    public static TBuilder AddObservability<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var telemetry = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing
                .AddSource("Azure.Messaging.ServiceBus")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            telemetry.UseOtlpExporter();
        return builder;
    }

    public static IServiceCollection AddDefaultApiServices(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<DefaultApiExceptionHandler>();
        services.AddHealthChecks();
        return services;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready");
        return app;
    }
}

public sealed class DefaultApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails details;
        if (exception is ValidationException validation)
        {
            details = new HttpValidationProblemDetails(validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).Distinct().ToArray()))
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
            };
        }
        else
        {
            var (status, title) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Operation conflict"),
                HttpRequestException => (StatusCodes.Status502BadGateway, "Upstream service failed"),
                _ => (StatusCodes.Status500InternalServerError, "Server error"),
            };
            details = new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = status < 500 ? exception.Message : null,
            };
        }

        details.Instance = context.Request.Path;
        context.Response.StatusCode = details.Status!.Value;
        await context.Response.WriteAsJsonAsync(details, cancellationToken);
        return true;
    }
}
