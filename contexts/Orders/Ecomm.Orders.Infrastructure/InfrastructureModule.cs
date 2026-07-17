using System.Text;

using Ecomm.Messaging;
using Ecomm.Orders.Application.Abstractions;
using Ecomm.Orders.Infrastructure.Authentication;
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
    public static IServiceCollection AddWorkerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));
        services.AddSingleton(TimeProvider.System);
        services.AddServiceBusTransport(configuration);
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        services.AddServiceBusTransport(configuration);
        services.AddSingleton<IOutboxStore, EfOutboxStore<OrdersDbContext>>();
        services.AddHostedService<OutboxDispatcher>();

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
