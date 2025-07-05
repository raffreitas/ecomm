using Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Products.Infrastructure.Repositories;

namespace Ecomm.Products.WebApi.Features.Products;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<AddProductHandler>();

        return services;
    }
}
