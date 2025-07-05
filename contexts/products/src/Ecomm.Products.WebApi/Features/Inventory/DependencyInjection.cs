namespace Ecomm.Products.WebApi.Features.Inventory;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryFeature(this IServiceCollection services)
    {
        // Add inventory-specific services here
        return services;
    }
}
