using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductsPaged;

public sealed class GetProductsPagedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/products", Handle)
            .WithTags("Products")
            .WithName("GetProductsPaged")
            .Produces<PagedResult<ProductSummaryResponse>>(200)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to retrieve a paginated list of products.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        [AsParameters] GetProductsPagedQuery query,
        GetProductsPagedHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    }
}
