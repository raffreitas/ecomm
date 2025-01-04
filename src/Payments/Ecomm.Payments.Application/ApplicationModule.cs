using Microsoft.Extensions.DependencyInjection;

namespace Ecomm.Payments.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediator();
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));
        return services;
    }
}