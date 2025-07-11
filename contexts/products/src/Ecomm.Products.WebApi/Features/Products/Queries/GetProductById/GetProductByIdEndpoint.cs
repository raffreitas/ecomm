using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/products/{id:guid}", Handle)
            .WithTags("Products")
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(200)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to retrieve a product by its ID.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(Guid id, GetProductByIdHandler handler, CancellationToken ct)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await handler.Handle(query, ct);

        return Results.Ok(result);
    }
}
