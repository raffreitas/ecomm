using System.Text.Json;

using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Domain.Entities;
using Ecomm.Catalog.Features.Products.CreateProduct;

using CreateProductHandler = Ecomm.Catalog.Features.Products.CreateProduct.Handler;
using CreateProductRequest = Ecomm.Catalog.Features.Products.CreateProduct.Request;
using CreateProductValidator = Ecomm.Catalog.Features.Products.CreateProduct.Validator;
using GetProductByIdHandler = Ecomm.Catalog.Features.Products.GetProductById.Handler;
using GetProductsByCategoryHandler = Ecomm.Catalog.Features.Categories.GetProducts.Handler;
using GetProductsHandler = Ecomm.Catalog.Features.Products.GetProducts.Handler;

namespace Ecomm.Catalog.UnitTests;

public sealed class ProductHandlerTests
{
    [Fact]
    public async Task Create_persists_product_before_publishing_compatible_event()
    {
        await using var dbContext = TestDb.Create();
        var category = new Category { Name = "Electronics" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        var messageBus = new FakeMessageBus(() => dbContext.Products.Any());

        var response = await CreateProductHandler.ExecuteAsync(
            ValidRequest(category.Id),
            new CreateProductValidator(),
            dbContext,
            messageBus);

        var product = Assert.Single(dbContext.Products);
        Assert.Equal(product.Id, response.Id);
        Assert.Equal(category.Id, response.Category.Id);
        Assert.True(messageBus.WasStatePersistedAtPublish);
        Assert.Equal(CreateProductHandler.ProductCreatedQueue, messageBus.Queue);

        var integrationEvent = JsonSerializer.Deserialize<ProductCreatedIntegrationEvent>(messageBus.Message!);
        Assert.NotNull(integrationEvent);
        Assert.Equal(product.Id, integrationEvent.Id);
        Assert.Equal(product.Name, integrationEvent.Name);
        Assert.Equal(product.Price, integrationEvent.Price);
        Assert.Equal(category.Id, integrationEvent.CategoryId);
    }

    [Fact]
    public async Task Create_rejects_unknown_category_without_persisting_or_publishing()
    {
        await using var dbContext = TestDb.Create();
        var messageBus = new FakeMessageBus();

        await Assert.ThrowsAsync<NotFoundException>(() => CreateProductHandler.ExecuteAsync(
            ValidRequest(Guid.NewGuid()),
            new CreateProductValidator(),
            dbContext,
            messageBus));

        Assert.Empty(dbContext.Products);
        Assert.Null(messageBus.Message);
    }

    [Fact]
    public async Task Broker_failure_does_not_rollback_persisted_product()
    {
        await using var dbContext = TestDb.Create();
        var category = new Category { Name = "Electronics" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        var messageBus = new FakeMessageBus { ThrowOnPublish = true };

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateProductHandler.ExecuteAsync(
            ValidRequest(category.Id),
            new CreateProductValidator(),
            dbContext,
            messageBus));

        Assert.Single(dbContext.Products);
    }

    [Fact]
    public async Task Product_queries_return_nested_category()
    {
        await using var dbContext = TestDb.Create();
        var (category, product) = await SeedProductAsync(dbContext);

        var all = await GetProductsHandler.ExecuteAsync(dbContext);
        var byId = await GetProductByIdHandler.ExecuteAsync(product.Id, dbContext);
        var byCategory = await GetProductsByCategoryHandler.ExecuteAsync(category.Id, dbContext);

        Assert.Single(all);
        Assert.Equal(category.Name, all[0].Category.Name);
        Assert.Equal(product.Id, byId.Id);
        Assert.Equal(category.Id, byId.Category.Id);
        Assert.Single(byCategory);
        Assert.Equal(product.Id, byCategory[0].Id);
    }

    [Fact]
    public async Task Product_queries_distinguish_empty_collection_from_missing_resource()
    {
        await using var dbContext = TestDb.Create();
        var category = new Category { Name = "Empty category" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var products = await GetProductsByCategoryHandler.ExecuteAsync(category.Id, dbContext);

        Assert.Empty(products);
        await Assert.ThrowsAsync<NotFoundException>(
            () => GetProductsByCategoryHandler.ExecuteAsync(Guid.NewGuid(), dbContext));
        await Assert.ThrowsAsync<NotFoundException>(
            () => GetProductByIdHandler.ExecuteAsync(Guid.NewGuid(), dbContext));
    }

    private static CreateProductRequest ValidRequest(Guid categoryId)
    {
        return new CreateProductRequest(
            "Laptop",
            "A developer laptop",
            5000,
            "https://example.com/laptop.png",
            categoryId);
    }

    private static async Task<(Category Category, Product Product)> SeedProductAsync(
        Ecomm.Catalog.Infrastructure.Persistence.CatalogDbContext dbContext)
    {
        var category = new Category { Name = "Electronics" };
        var product = new Product
        {
            Name = "Laptop",
            Description = "A developer laptop",
            Price = 5000,
            ImageUrl = "https://example.com/laptop.png",
            CategoryId = category.Id,
            Category = category,
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return (category, product);
    }
}
