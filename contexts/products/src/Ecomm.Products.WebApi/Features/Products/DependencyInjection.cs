using Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;
using Ecomm.Products.WebApi.Features.Products.Commands.UpdateProduct;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Products.Infrastructure.Repositories;
using Ecomm.Products.WebApi.Features.Products.Queries.GetProductById;
using Ecomm.Products.WebApi.Features.Products.Queries.GetProductsPaged;
using Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

namespace Ecomm.Products.WebApi.Features.Products;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<AddProductHandler>();
        services.AddScoped<UpdateProductHandler>();

        services.AddScoped<GetProductByIdHandler>();
        services.AddScoped<GetProductsPagedHandler>();
        services.AddScoped<SearchProductsHandler>();

        return services;
    }
}
