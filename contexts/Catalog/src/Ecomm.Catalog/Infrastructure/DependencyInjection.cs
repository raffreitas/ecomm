using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Features.Categories.CreateCategory;
using Ecomm.Catalog.Infrastructure.Messaging;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));
        services.AddScoped<IMessageBusService, RabbitMqMessageBusService>();

        return services;
    }

    public static IServiceCollection AddFeatureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Validator>();
        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        context.Database.Migrate();
    }
}
