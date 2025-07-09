using Ecomm.Products.WebApi.Shared.Infrastructure.AI.Settings;

using Microsoft.SemanticKernel;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddSemanticKernelConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SemanticKernelSettings>()
            .BindConfiguration(SemanticKernelSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration
            .GetSection(SemanticKernelSettings.SectionName)
            .Get<SemanticKernelSettings>()!;

        services.AddSingleton(sp =>
        {
            var kernel = Kernel.CreateBuilder();
            kernel.AddOpenAIChatCompletion(
                settings.ModelName,
                settings.ApiKey
            );
            return kernel.Build();
        });

        return services;
    }
}
