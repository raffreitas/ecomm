using Ecomm.Products.WebApi.Shared.Infrastructure.Messaging;

namespace Ecomm.Products.WebApi.Shared.Workers;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedWorkers(this IServiceCollection services)
    {
        services.AddHostedService<OutboxEventPublisherWorker>();
        services.AddScoped<IIntegrationEventMessageMapper, IntegrationEventMessageMapper>();
        return services;
    }
}
