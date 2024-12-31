using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Orders.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddMediator();
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));
        return services;
    }
}