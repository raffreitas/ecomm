using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/categories/{id:guid}", Handle)
            .WithTags("Categories")
            .WithName("UpdateCategory")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to update a category, including changing its parent.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        Guid id,
        UpdateCategoryRequest request,
        UpdateCategoryHandler handler,
        CancellationToken ct)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Name = request.Name,
            ParentCategoryId = request.ParentCategoryId
        };
        await handler.Handle(command, ct);
        return Results.NoContent();
    }
}

public sealed record UpdateCategoryRequest
{
    public required string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }
}
