using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.AddStock;

public class AddStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/inventory/{productId:guid}/add-stock", Handle)
            .WithTags("Inventory")
            .WithName("AddStock")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to add stock to a product inventory.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        AddStockRequest request,
        AddStockHandler handler,
        CancellationToken ct)
    {
        var command = new AddStockCommand
        {
            ProductId = productId,
            Quantity = request.Quantity
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record AddStockRequest
{
    public required int Quantity { get; init; }
}
