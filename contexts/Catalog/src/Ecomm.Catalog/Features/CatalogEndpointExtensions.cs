using Ecomm.Catalog.Features.Categories.CreateCategory;
using Ecomm.Catalog.Features.Categories.GetCategories;
using Ecomm.Catalog.Features.Categories.GetCategoryById;
using Ecomm.Catalog.Features.Categories.GetProducts;
using Ecomm.Catalog.Features.Products.CreateProduct;
using Ecomm.Catalog.Features.Products.GetProductById;
using Ecomm.Catalog.Features.Products.GetProducts;

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
