using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Commands.UpdateProduct;

public class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/products/{id:guid}", Handle)
            .WithTags("Products")
            .WithName("UpdateProduct")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to update an existing product.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid id,
        UpdateProductRequest request,
        UpdateProductHandler handler,
        CancellationToken ct)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            Categories = request.Categories
        };

        await handler.Handle(command, ct);

        return Results.NoContent();
    }
}

public sealed record UpdateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
    public required string[] Categories { get; init; } = [];
}
