using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;

public sealed class AddProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/products", Handle)
            .WithTags("Products")
            .WithName("AddProduct")
            .Produces(201)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to add a new product to the catalog.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(AddProductCommand command, AddProductHandler handler, CancellationToken ct)
    {
        await handler.Handle(command, ct);
        return Results.Created();
    }
}
