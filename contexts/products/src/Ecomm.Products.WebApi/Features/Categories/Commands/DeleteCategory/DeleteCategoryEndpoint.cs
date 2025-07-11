using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .WithName("DeleteCategory")
            .Produces(204)
            .Produces(404)
            .WithDescription("This endpoint allows you to delete a category.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid id,
        DeleteCategoryHandler handler,
        CancellationToken ct)
    {
        var command = new DeleteCategoryCommand { Id = id };
        await handler.Handle(command, ct);
        return Results.NoContent();
    }
}
