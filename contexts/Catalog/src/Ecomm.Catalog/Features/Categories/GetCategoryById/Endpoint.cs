using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetCategoryById;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync);
    }

    private static async Task<Ok<CategoryResponse>> HandleAsync(
        Guid id,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Handler.ExecuteAsync(id, dbContext, cancellationToken));
    }
}

public static class Handler
{
    public static async Task<CategoryResponse> ExecuteAsync(
        Guid id,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync(cancellationToken);

        return category ?? throw new NotFoundException("Category not found");
    }
}
