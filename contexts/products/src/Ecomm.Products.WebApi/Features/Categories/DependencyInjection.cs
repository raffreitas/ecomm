using Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;
using Ecomm.Products.WebApi.Features.Categories.Commands.UpdateCategory;
using Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Categories.Infrastructure.Repositories;
using Ecomm.Products.WebApi.Features.Categories.Queries.GetCategories;

namespace Ecomm.Products.WebApi.Features.Categories;

public static class DependencyInjection
{
    public static IServiceCollection AddCategoriesFeature(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Command handlers
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();

        // Query handlers
        services.AddScoped<GetCategoriesHandler>();

        return services;
    }
}
