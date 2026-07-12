using System.Text.Json;

using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Domain.Entities;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.CreateProduct;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("", HandleAsync);
    }

    private static async Task<Created<ProductResponse>> HandleAsync(
        Request request,
        IValidator<Request> validator,
        CatalogDbContext dbContext,
        IMessageBusService messageBus,
        CancellationToken cancellationToken)
    {
        var response = await Handler.ExecuteAsync(request, validator, dbContext, messageBus, cancellationToken);
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

public static class Handler
{
    public const string ProductCreatedQueue = "product.created";

    public static async Task<ProductResponse> ExecuteAsync(
        Request request,
        IValidator<Request> validator,
        CatalogDbContext dbContext,
        IMessageBusService messageBus,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == request.CategoryId)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId,
        };

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var integrationEvent = new ProductCreatedIntegrationEvent(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            product.CategoryId);

        await messageBus.PublishAsync(
            ProductCreatedQueue,
            JsonSerializer.SerializeToUtf8Bytes(integrationEvent),
            cancellationToken);

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            category);
    }
}

public sealed record ProductCreatedIntegrationEvent(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId);
