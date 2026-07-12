using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Infrastructure.Messaging;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Ecomm.Catalog.Features.Categories.CreateCategory;
using Ecomm.Catalog.Features.Categories.GetCategories;
using Ecomm.Catalog.Features.Categories.GetCategoryById;
using Ecomm.Catalog.Features.Categories.GetProducts;
using Ecomm.Catalog.Features.Products.CreateProduct;
using Ecomm.Catalog.Features.Products.GetProductById;
using Ecomm.Catalog.Features.Products.GetProducts;

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
        services.AddValidatorsFromAssemblyContaining<
            global::Ecomm.Catalog.Features.Categories.CreateCategory.Validator>();
        return services;
    }

    public static IServiceCollection AddFeatureHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<GetCategoriesHandler>();
        services.AddScoped<GetCategoryByIdHandler>();
        services.AddScoped<GetProductsByCategoryHandler>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<GetProductByIdHandler>();
        services.AddScoped<GetProductsHandler>();

        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        context.Database.Migrate();
    }
}
