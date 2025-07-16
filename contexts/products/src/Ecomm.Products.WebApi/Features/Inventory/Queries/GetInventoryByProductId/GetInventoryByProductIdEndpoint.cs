using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoryByProductId;

public sealed class GetInventoryByProductIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/inventory/product/{productId:guid}", Handle)
            .WithTags("Inventory")
            .WithName("GetInventoryByProductId")
            .Produces<GetInventoryByProductIdResponse>(200)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to retrieve inventory information by product ID.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(Guid productId, GetInventoryByProductIdHandler handler, CancellationToken ct)
    {
        var query = new GetInventoryByProductIdQuery { ProductId = productId };
        var result = await handler.Handle(query, ct);
        
        return result is null 
            ? Results.NotFound() 
            : Results.Ok(result);
    }
}
