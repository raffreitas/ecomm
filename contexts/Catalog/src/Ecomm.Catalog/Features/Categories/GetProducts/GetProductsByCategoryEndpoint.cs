using Ecomm.Catalog.Features.Contracts;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Categories.GetProducts;

public static class GetProductsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{categoryId:guid}/products", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> HandleAsync(
        Guid categoryId,
        GetProductsByCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.ExecuteAsync(categoryId, cancellationToken));
    }
}
