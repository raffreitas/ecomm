using Microsoft.Extensions.DependencyInjection;

using FluentValidation;

namespace Ecomm.Orders.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddMediator().AddFluentValidation();
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));
        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationModule).Assembly, includeInternalTypes: true);
        return services;
    }
}