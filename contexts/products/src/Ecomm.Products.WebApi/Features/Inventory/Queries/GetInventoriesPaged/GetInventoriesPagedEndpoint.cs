using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoriesPaged;

public class GetInventoriesPagedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/inventory", Handle)
            .WithTags("Inventory")
            .WithName("GetInventoriesPaged")
            .Produces<PagedResult<InventorySummaryResponse>>(200)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to retrieve a paginated list of inventories.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        [AsParameters] GetInventoriesPagedQuery query,
        GetInventoriesPagedHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    }
}
