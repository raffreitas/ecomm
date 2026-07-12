using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetCategories;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<CategoryResponse>>> HandleAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Handler.ExecuteAsync(dbContext, cancellationToken));
    }
}

public static class Handler
{
    public static async Task<IReadOnlyList<CategoryResponse>> ExecuteAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .ToListAsync(cancellationToken);
    }
}
