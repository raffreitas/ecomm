using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Categories.Queries.GetCategories;

public class GetCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/categories", Handle)
            .WithTags("Categories")
            .WithName("GetCategories")
            .Produces<List<CategoryDto>>(200)
            .WithDescription("This endpoint allows you to list all categories or subcategories by parent.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        [AsParameters] GetCategoriesQuery query,
        GetCategoriesHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    }
}
