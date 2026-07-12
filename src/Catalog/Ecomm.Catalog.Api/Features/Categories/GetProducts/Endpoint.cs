using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Categories.GetProducts;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{categoryId:guid}/products", HandleAsync);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> HandleAsync(
        Guid categoryId,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Handler.ExecuteAsync(categoryId, dbContext, cancellationToken));
    }
}

public static class Handler
{
    public static async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(
        Guid categoryId,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == categoryId)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }

        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.CategoryId == categoryId)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl,
                category))
            .ToListAsync(cancellationToken);
    }
}
