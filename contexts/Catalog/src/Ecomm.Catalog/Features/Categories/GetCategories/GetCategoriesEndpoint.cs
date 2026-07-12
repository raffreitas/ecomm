using Ecomm.Catalog.Features.Contracts;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Categories.GetCategories;

public static class GetCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<CategoryResponse>>> HandleAsync(
        GetCategoriesHandler handler,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.ExecuteAsync(cancellationToken));
    }
}
