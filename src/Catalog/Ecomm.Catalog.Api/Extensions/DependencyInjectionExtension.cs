using Ecomm.Catalog.Endpoints;
using Ecomm.Catalog.Messaging;
using Ecomm.Catalog.Persistence;
using Ecomm.Catalog.Persistence.Repositories;
using Ecomm.Catalog.Repositories;
using Ecomm.Catalog.Services;
using Ecomm.Catalog.Services.Contracts;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DatabaseConnection")));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusService, RabbitMqMessageBusService>();
        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetService<CatalogDbContext>();
        context!.Database.Migrate();
    }

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProductEndpoints();
        app.MapCategoryEndpoints();
    }
}