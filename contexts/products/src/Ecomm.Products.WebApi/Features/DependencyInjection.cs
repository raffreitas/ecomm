using Ecomm.Products.WebApi.Features.Categories;
using Ecomm.Products.WebApi.Features.Inventory;
using Ecomm.Products.WebApi.Features.Products;
using Ecomm.Products.WebApi.Shared.Domain.Events;

namespace Ecomm.Products.WebApi.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeaturesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddCategoriesFeature();
        services.AddProductsFeature();
        services.AddInventoryFeature();
        return services;
    }
}
