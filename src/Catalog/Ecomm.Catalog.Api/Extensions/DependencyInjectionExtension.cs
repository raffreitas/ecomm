﻿using Ecomm.Catalog.Api.Endpoints;
using Ecomm.Catalog.Api.Persistence;
using Ecomm.Catalog.Api.Persistence.Repositories;
using Ecomm.Catalog.Api.Repositories;
using Ecomm.Catalog.Api.Services;
using Ecomm.Catalog.Api.Services.Contracts;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Api.Extensions;

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

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProductEndpoints();
        app.MapCategoryEndpoints();
    }
}