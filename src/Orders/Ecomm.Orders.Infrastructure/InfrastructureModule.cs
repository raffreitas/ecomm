using System.Text;

using Ecomm.Orders.Application.Abstractions;
using Ecomm.Orders.Domain.Repositories;
using Ecomm.Orders.Infrastructure.Authentication;
using Ecomm.Orders.Infrastructure.MessageBus;
using Ecomm.Orders.Infrastructure.MessageBus.Consumers;
using Ecomm.Orders.Infrastructure.Persistence;
using Ecomm.Orders.Infrastructure.Persistence.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Ecomm.Orders.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddMessageBus()
            .AddMessageConsumers()
            .AddAuthentication(configuration);
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));

        return services;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusService, RabbitMqMessageBusService>();
        return services;
    }

    private static IServiceCollection AddMessageConsumers(this IServiceCollection services)
    {
        services.AddHostedService<CustomerCreatedConsumer>();
        services.AddHostedService<ProductCreatedConsumer>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        var secretKey = configuration["JWT:SecretKey"]!;

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            });

        return services;
    }
}