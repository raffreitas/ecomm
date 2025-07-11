using Ecomm.Shared.Infrastructure.Messaging.Abstractions;
using Ecomm.Shared.Infrastructure.Messaging.Providers.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace Ecomm.Shared.Infrastructure.Messaging;

public static class DependencyInjection
{
    public const string ConnectionStringName = "RabbitMqConnection";
    public static IServiceCollection AddMessagingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.UseRabbitMQ(configuration);
        return services;
    }

    private static void UseRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)!;

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        var channelFactoryTask = ChannelFactory.CreateAsync(connectionFactory);
        channelFactoryTask.Wait();
        var channelFactory = channelFactoryTask.Result;

        services.AddTransient(_ => channelFactory);

        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        services.AddSingleton<IMessageConsumer, RabbitMqMessageConsumer>();
    }
}
