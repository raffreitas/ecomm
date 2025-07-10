using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReserveStock;

public class ReserveStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/inventory/{productId:guid}/reserve", Handle)
            .WithTags("Inventory")
            .WithName("ReserveStock")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to reserve stock for a product.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        ReserveStockRequest request,
        ReserveStockHandler handler,
        CancellationToken ct)
    {
        var command = new ReserveStockCommand
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record ReserveStockRequest
{
    public required int Quantity { get; init; }
}
