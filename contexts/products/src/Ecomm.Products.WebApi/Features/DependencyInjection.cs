using Ecomm.Products.WebApi.Features.Inventory;
using Ecomm.Products.WebApi.Features.Products;

namespace Ecomm.Products.WebApi.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeaturesConfiguration(this IServiceCollection services)
    {
        services.AddProductsFeature();
        services.AddInventoryFeature();
        return services;
    }
}
