using Ecomm.Products.WebApi.Shared.Presentation.Endpoints;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;

public class CreateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/categories", Handle)
            .WithTags("Categories")
            .WithName("CreateCategory")
            .Produces<CreateCategoryResponse>(201)
            .ProducesValidationProblem()
            .WithDescription("This endpoint allows you to create a new category, optionally as a subcategory.")
            .WithOpenApi();
    }

    private async Task<IResult> Handle(
        CreateCategoryCommand command,
        CreateCategoryHandler handler,
        CancellationToken ct)
    {
        var id = await handler.Handle(command, ct);
        return Results.Created($"api/v1/categories/{id}", new CreateCategoryResponse { Id = id });
    }
}

public sealed record CreateCategoryResponse
{
    public required Guid Id { get; init; }
}
