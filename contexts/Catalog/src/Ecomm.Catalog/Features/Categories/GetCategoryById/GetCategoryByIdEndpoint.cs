using Ecomm.Catalog.Features.Contracts;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Categories.GetCategoryById;

public static class GetCategoryByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync);
    }

    private static async Task<Ok<CategoryResponse>> HandleAsync(
        Guid id,
        GetCategoryByIdHandler handler,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.ExecuteAsync(id, cancellationToken));
    }
}
