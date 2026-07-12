using Ecomm.Catalog.Features.Contracts;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Categories.CreateCategory;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("", HandleAsync);
    }

    private static async Task<Created<CategoryResponse>> HandleAsync(
        Request request,
        CreateCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.ExecuteAsync(request, cancellationToken);
        return TypedResults.Created($"/api/v1/categories/{response.Id}", response);
    }
}

public sealed record Request(string Name);

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(request => request.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name must not be empty.")
            .MaximumLength(100);
    }
}
