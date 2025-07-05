using System.Reflection;

using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;
using Ecomm.Products.WebApi.Shared.Presentation.OpenApi;

namespace Ecomm.Products.WebApi.Shared.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationConfiguration(this IServiceCollection services)
    {
        services.AddEndpoints(Assembly.GetExecutingAssembly());
        services.AddOpenApiConfiguration();
        return services;
    }
    public static IApplicationBuilder UsePresentationConfiguration(this WebApplication app)
    {
        app.MapEndpoints();
        app.UseOpenApiConfiguration();
        app.UseHttpsRedirection();
        return app;
    }
}
