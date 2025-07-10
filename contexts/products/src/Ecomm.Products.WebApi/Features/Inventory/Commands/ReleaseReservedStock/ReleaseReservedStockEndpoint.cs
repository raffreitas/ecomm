using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ReleaseReservedStock;

public class ReleaseReservedStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/inventory/{productId:guid}/release-reserve", Handle)
            .WithTags("Inventory")
            .WithName("ReleaseReservedStock")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to release reserved stock for a product.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        ReleaseReservedStockRequest request,
        ReleaseReservedStockHandler handler,
        CancellationToken ct)
    {
        var command = new ReleaseReservedStockCommand
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record ReleaseReservedStockRequest
{
    public required int Quantity { get; init; }
}
