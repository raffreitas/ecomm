using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.RemoveStock;

public sealed class RemoveStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/inventory/{productId:guid}/remove-stock", Handle)
            .WithTags("Inventory")
            .WithName("RemoveStock")
            .Produces(204)
            .Produces(404)
            .Produces(409)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to remove stock from a product inventory.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        RemoveStockRequest request,
        RemoveStockHandler handler,
        CancellationToken ct)
    {
        var command = new RemoveStockCommand
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record RemoveStockRequest
{
    public required int Quantity { get; init; }
}
