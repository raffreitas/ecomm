using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;

public sealed class ActivateInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/inventory/{productId:guid}/activate", Handle)
            .WithTags("Inventory")
            .WithName("ActivateInventory")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to activate an inventory item.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        ActivateInventoryHandler handler,
        CancellationToken ct)
    {
        var command = new ActivateInventoryCommand { ProductId = productId };
        await handler.Handle(command, ct);
        return Results.NoContent();
    }
}
