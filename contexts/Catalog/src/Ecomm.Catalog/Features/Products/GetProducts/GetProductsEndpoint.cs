using Ecomm.Catalog.Features.Contracts;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> HandleAsync(
        GetProductsHandler handler,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.ExecuteAsync(cancellationToken));
    }
}
