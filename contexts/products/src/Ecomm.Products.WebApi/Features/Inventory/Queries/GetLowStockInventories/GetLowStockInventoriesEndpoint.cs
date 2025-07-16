using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetLowStockInventories;

public sealed class GetLowStockInventoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/inventory/low-stock", Handle)
            .WithTags("Inventory")
            .WithName("GetLowStockInventories")
            .Produces<PagedResult<LowStockInventoryResponse>>(200)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to retrieve inventories with low stock.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        [AsParameters] GetLowStockInventoriesQuery query,
        GetLowStockInventoriesHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(query, ct);
        return Results.Ok(result);
    }
}
