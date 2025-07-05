using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

public class SearchProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/products/search", Handle)
            .WithTags("Products")
            .WithName("SearchProducts")
            .Produces<PagedResult<SearchProductsResponse>>(200)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to search for products by name, description, or price.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        [AsParameters] SearchProductsQuery query,
        SearchProductsHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    }
}
