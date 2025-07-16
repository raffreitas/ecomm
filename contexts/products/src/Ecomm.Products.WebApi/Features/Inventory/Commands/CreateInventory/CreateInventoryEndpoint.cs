using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Inventory.Commands.CreateInventory;

public sealed class CreateInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/inventory", Handle)
            .WithTags("Inventory")
            .WithName("CreateInventory")
            .Produces<CreateInventoryResponse>(201)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to create inventory for a product.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(CreateInventoryCommand command, CreateInventoryHandler handler, CancellationToken ct)
    {
        var inventoryId = await handler.Handle(command, ct);
        var response = new CreateInventoryResponse { Id = inventoryId };
        return Results.Created($"api/v1/inventory/{inventoryId}", response);
    }
}

public sealed record CreateInventoryResponse
{
    public required Guid Id { get; init; }
}
