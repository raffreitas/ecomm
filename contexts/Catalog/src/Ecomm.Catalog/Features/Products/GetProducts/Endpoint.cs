using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.GetProducts;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> HandleAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Handler.ExecuteAsync(dbContext, cancellationToken));
    }
}

public static class Handler
{
    public static async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                new CategoryResponse(product.CategoryId, product.Category!.Name)))
            .ToListAsync(cancellationToken);
    }
}
