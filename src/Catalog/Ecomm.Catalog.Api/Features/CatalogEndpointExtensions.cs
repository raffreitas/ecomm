using CreateCategoryEndpoint = Ecomm.Catalog.Features.Categories.CreateCategory.Endpoint;
using GetCategoriesEndpoint = Ecomm.Catalog.Features.Categories.GetCategories.Endpoint;
using GetCategoryByIdEndpoint = Ecomm.Catalog.Features.Categories.GetCategoryById.Endpoint;
using GetProductsByCategoryEndpoint = Ecomm.Catalog.Features.Categories.GetProducts.Endpoint;
using CreateProductEndpoint = Ecomm.Catalog.Features.Products.CreateProduct.Endpoint;
using GetProductByIdEndpoint = Ecomm.Catalog.Features.Products.GetProductById.Endpoint;
using GetProductsEndpoint = Ecomm.Catalog.Features.Products.GetProducts.Endpoint;

namespace Ecomm.Catalog.Features;

public static class CatalogEndpointExtensions
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1");
        var categories = api.MapGroup("/categories");
        var products = api.MapGroup("/products");

        CreateCategoryEndpoint.Map(categories);
        GetCategoriesEndpoint.Map(categories);
        GetCategoryByIdEndpoint.Map(categories);
        GetProductsByCategoryEndpoint.Map(categories);

        CreateProductEndpoint.Map(products);
        GetProductsEndpoint.Map(products);
        GetProductByIdEndpoint.Map(products);

        return app;
    }
}
