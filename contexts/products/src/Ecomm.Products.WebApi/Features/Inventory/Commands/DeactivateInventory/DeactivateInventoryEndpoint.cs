using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.DeactivateInventory;

public sealed class DeactivateInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/inventory/{productId:guid}/deactivate", Handle)
            .WithTags("Inventory")
            .WithName("DeactivateInventory")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to deactivate an inventory item.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        DeactivateInventoryHandler handler,
        CancellationToken ct)
    {
        var command = new DeactivateInventoryCommand { ProductId = productId };
        await handler.Handle(command, ct);
        return Results.NoContent();
    }
}
