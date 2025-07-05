namespace Ecomm.Products.WebApi.Features.Products;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        // Add product-specific services here
        return services;
    }
}
