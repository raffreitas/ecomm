using Ecomm.Shared.Infrastructure.EventSourcing.Abstractions;
using Ecomm.Shared.Infrastructure.EventSourcing.Providers.KurrentDb;

using KurrentDB.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Shared.Infrastructure.EventSourcing;

public static class DependencyInjection
{
    private const string ConnectionStringName = "EventStoreConnection";
    public static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
    {
        services.UseKurrentDb(configuration);
        return services;
    }

    private static IServiceCollection UseKurrentDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringName)!;
            var settings = KurrentDBClientSettings.Create(connectionString);
            return new KurrentDBClient(settings);
        });

        services.AddScoped<IEventStoreService, KurrentDbService>();

        return services;
    }
}
