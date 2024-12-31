using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Services.Contracts;

namespace Ecomm.Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products");
        group.MapPost("", CreateProductAsync);
        group.MapGet("", GetProductsAsync);
        group.MapGet("{id:guid}", GetProductByIdAsync);
        group.MapGet("categories/{categoryId:guid}", GetProductByCategoryIdAsync);
        
        return app;
    }

    private static async Task<IResult> CreateProductAsync(
        this IProductService service,
        CreateProductInputModel inputModel,
        CancellationToken cancellationToken)
    {
        await service.CreateProductAsync(inputModel, cancellationToken);
        return Results.Created();
    }

    private static async Task<IResult> GetProductsAsync(
        this IProductService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetProductsAsync(cancellationToken));
    }

    private static async Task<IResult> GetProductByIdAsync(Guid id, IProductService service)
    {
        return Results.Ok(await service.GetProductByIdAsync(id));
    }

    private static async Task<IResult> GetProductByCategoryIdAsync(Guid categoryId, IProductService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetProductByCategoryIdAsync(categoryId, cancellationToken));
    }
}