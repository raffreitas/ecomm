using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Domain.Entities;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Categories.CreateCategory;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("", HandleAsync);
    }

    private static async Task<Created<CategoryResponse>> HandleAsync(
        Request request,
        IValidator<Request> validator,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var response = await Handler.ExecuteAsync(request, validator, dbContext, cancellationToken);
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

public static class Handler
{
    public static async Task<CategoryResponse> ExecuteAsync(
        Request request,
        IValidator<Request> validator,
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var category = new Category { Name = request.Name };
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name);
    }
}
