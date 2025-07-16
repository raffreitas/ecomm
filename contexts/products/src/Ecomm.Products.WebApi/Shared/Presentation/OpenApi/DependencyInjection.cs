using Scalar.AspNetCore;

namespace Ecomm.Products.WebApi.Shared.Presentation.OpenApi;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();

        return services;
    }
    public static IApplicationBuilder UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}
