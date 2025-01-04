using Ecomm.Payments.Domain.Repositories;
using Ecomm.Payments.Domain.Services;
using Ecomm.Payments.Infrastructure.MessageBus.Consumers;
using Ecomm.Payments.Infrastructure.Payments;
using Ecomm.Payments.Infrastructure.Persistence;
using Ecomm.Payments.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace Ecomm.Payments.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddRabbitMq(configuration)
            .AddPayment(configuration);
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));
        });
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<OrderCreatedConsumer>();
        return services;
    }

    private static IServiceCollection AddPayment(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        return services;
    }
}