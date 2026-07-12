using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Common.Messaging;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecomm.Catalog.Features.Products.CreateProduct;

public static class CreateProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("", HandleAsync);
    }

    private static async Task<Created<ProductResponse>> HandleAsync(
        Request request,
        CreateProductHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.ExecuteAsync(request, cancellationToken);
        return TypedResults.Created($"/api/v1/products/{response.Id}", response);
    }
}

public sealed record Request(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId);

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(request => request.Name)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Name must not be empty.")
            .MaximumLength(100);

        RuleFor(request => request.Description)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Description must not be empty.")
            .MaximumLength(1000);

        RuleFor(request => request.Price).GreaterThan(0);

        RuleFor(request => request.ImageUrl)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("ImageUrl must not be empty.")
            .MaximumLength(100)
            .Must(BeAbsoluteHttpUrl)
            .WithMessage("ImageUrl must be an absolute HTTP or HTTPS URL.");

        RuleFor(request => request.CategoryId).NotEmpty();
    }

    private static bool BeAbsoluteHttpUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

public sealed record ProductCreatedIntegrationEvent(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId) : IIntegrationEvent;
