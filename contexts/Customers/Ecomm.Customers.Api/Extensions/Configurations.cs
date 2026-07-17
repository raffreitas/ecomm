using System.Reflection;

using Ecomm.Customers.Api.Features.CreateCustomer;
using Ecomm.Customers.Api.Persistence;
using Ecomm.Messaging;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Extensions;

public static class Configurations
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(option =>
            option.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));
        services.AddSingleton(TimeProvider.System);
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        services.AddServiceBusTransport(configuration);
        services.AddSingleton<IOutboxStore, EfOutboxStore<CustomersDbContext>>();
        services.AddHostedService<OutboxDispatcher>();
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<CreateCustomerHandler>();
        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        dbContext.Database.Migrate();
    }
}
