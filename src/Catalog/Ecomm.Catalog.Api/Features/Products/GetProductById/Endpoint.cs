using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.GetProductById;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync);
    }

    private static async Task<Ok<ProductResponse>> HandleAsync(
        Guid id,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Handler.ExecuteAsync(id, dbContext, cancellationToken));
    }
}

public static class Handler
{
    public static async Task<ProductResponse> ExecuteAsync(
        Guid id,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                new CategoryResponse(product.CategoryId, product.Category!.Name)))
            .SingleOrDefaultAsync(cancellationToken);

        return product ?? throw new NotFoundException("Product not found");
    }
}
