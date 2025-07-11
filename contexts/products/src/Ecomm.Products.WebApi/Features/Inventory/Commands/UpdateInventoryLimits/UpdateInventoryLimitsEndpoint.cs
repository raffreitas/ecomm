using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.UpdateInventoryLimits;

public sealed class UpdateInventoryLimitsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/inventory/{productId:guid}/update-limits", Handle)
            .WithTags("Inventory")
            .WithName("UpdateInventoryLimits")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to update minimum and maximum stock levels for an inventory item.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        UpdateInventoryLimitsRequest request,
        UpdateInventoryLimitsHandler handler,
        CancellationToken ct)
    {
        var command = new UpdateInventoryLimitsCommand
        {
            ProductId = productId,
            MinimumStockLevel = request.MinimumStockLevel,
            MaximumStockLevel = request.MaximumStockLevel
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record UpdateInventoryLimitsRequest
{
    public required int MinimumStockLevel { get; init; }
    public required int MaximumStockLevel { get; init; }
}
