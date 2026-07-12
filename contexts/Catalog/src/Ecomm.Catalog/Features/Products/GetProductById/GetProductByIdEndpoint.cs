using Ecomm.Catalog.Features.Contracts;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Products.GetProductById;

public static class GetProductByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync);
    }

    private static async Task<Ok<ProductResponse>> HandleAsync(
        Guid id,
        GetProductByIdHandler handler,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.ExecuteAsync(id, cancellationToken));
    }
}
