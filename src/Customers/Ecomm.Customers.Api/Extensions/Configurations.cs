using System.Reflection;

using Ecomm.Customers.Api.Messaging;
using Ecomm.Customers.Api.Persistence;
using Ecomm.Customers.Api.Persistence.Repositories;
using Ecomm.Customers.Api.Repositories;
using Ecomm.Customers.Api.Services;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Customers.Api.Extensions;

public static class Configurations
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(option =>
            option.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IMessageBusService, RabbitMqMessageBusService>();

        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<CustomersDbContext>();
        dbContext.Database.Migrate();
    }
}