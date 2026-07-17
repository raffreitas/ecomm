using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusTransport(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(ServiceBusOptions.SectionName).Get<ServiceBusOptions>() ?? new();
        services.AddSingleton(_ => CreateClient(options));
        services.AddSingleton<IEventTransport, ServiceBusEventTransport>();
        return services;
    }

    private static ServiceBusClient CreateClient(ServiceBusOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            return new ServiceBusClient(options.ConnectionString);
        if (!string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace))
            return new ServiceBusClient(options.FullyQualifiedNamespace, new DefaultAzureCredential());

        throw new InvalidOperationException(
            $"Configure either {ServiceBusOptions.SectionName}:ConnectionString or FullyQualifiedNamespace.");
    }
}
