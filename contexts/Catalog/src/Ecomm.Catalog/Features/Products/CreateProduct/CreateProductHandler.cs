using System.Text.Json;

using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Common.Messaging;
using Ecomm.Catalog.Domain.Entities;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Catalog.Features.Products.CreateProduct;

public sealed class CreateProductHandler(
    IValidator<Request> validator,
    CatalogDbContext dbContext,
    IMessageBusService messageBus)
{
    public const string ProductCreatedQueue = "product.created";

    public async Task<ProductResponse> ExecuteAsync(
        Request request,
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
