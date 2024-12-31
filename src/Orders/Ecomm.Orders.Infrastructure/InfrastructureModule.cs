using Ecomm.Orders.Domain.Repositories;
using Ecomm.Orders.Infrastructure.Persistence.Repositories;


using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Orders.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services.AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}