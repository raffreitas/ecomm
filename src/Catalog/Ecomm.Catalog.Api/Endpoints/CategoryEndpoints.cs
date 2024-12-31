using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Services.Contracts;

namespace Ecomm.Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/categories");
        group.MapPost("", CreateCategoryAsync);
        group.MapGet("", GetCategoriesAsync);
        group.MapGet("{id:guid}", GetCategoryByIdAsync);

        return app;
    }

    private static async Task<IResult> CreateCategoryAsync(
        CreateCategoryInputModel inputModel,
        ICategoryService service,
        CancellationToken cancellationToken)
    {
        await service.CreateCategoryAsync(inputModel, cancellationToken);
        return Results.Created();
    }

    private static async Task<IResult> GetCategoriesAsync(ICategoryService service, CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetCategoriesAsync(cancellationToken));
    }

    private static async Task<IResult> GetCategoryByIdAsync(
        Guid id,
        ICategoryService service,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await service.GetCategoryByIdAsync(id, cancellationToken));
    }
}