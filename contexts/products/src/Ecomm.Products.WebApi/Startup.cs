using Aspire.ServiceDefaults;

using Ecomm.Products.WebApi.Features;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence;
using Ecomm.Products.WebApi.Shared.Infrastructure.SemanticKernel;
using Ecomm.Products.WebApi.Shared.Presentation;
using Ecomm.Shared.Infrastructure.Observability;

namespace Ecomm.Products.WebApi;

public static class Startup
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        // TODO: Add condition to use aspire only on development
        builder.AddServiceDefaults();

        services.AddPersistenceConfiguration(configuration);
        services.AddPresentationConfiguration();
        services.AddFeaturesConfiguration();
        services.AddSemanticKernelConfiguration(configuration);
        //services.AddObservabilityConfiguration(configuration);
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.MapDefaultEndpoints();

        app.UsePresentationConfiguration();
        app.UsePersistenceConfiguration();
        app.UseObservabilityConfiguration();
    }
}
