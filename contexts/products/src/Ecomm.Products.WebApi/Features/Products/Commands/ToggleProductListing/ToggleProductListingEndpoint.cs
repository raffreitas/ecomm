using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Commands.ToggleProductListing;

public sealed class ToggleProductListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1/products/{productId:guid}/listing", Handle)
            .WithTags("Products")
            .WithName("ToggleProductListing")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to activate or deactivate product listing, only if there is available stock.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid productId,
        ToggleProductListingRequest request,
        ToggleProductListingHandler handler,
        CancellationToken ct)
    {
        var command = new ToggleProductListingCommand
        {
            ProductId = productId,
            List = request.List
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record ToggleProductListingRequest
{
    public required bool List { get; init; }
}
