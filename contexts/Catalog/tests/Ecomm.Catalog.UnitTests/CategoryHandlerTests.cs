using Ecomm.Catalog.Common.Exceptions;
using Ecomm.Catalog.Domain.Entities;

using CreateCategoryHandler = Ecomm.Catalog.Features.Categories.CreateCategory.Handler;
using CreateCategoryRequest = Ecomm.Catalog.Features.Categories.CreateCategory.Request;
using CreateCategoryValidator = Ecomm.Catalog.Features.Categories.CreateCategory.Validator;
using GetCategoriesHandler = Ecomm.Catalog.Features.Categories.GetCategories.Handler;
using GetCategoryByIdHandler = Ecomm.Catalog.Features.Categories.GetCategoryById.Handler;

namespace Ecomm.Catalog.UnitTests;

public sealed class CategoryHandlerTests
{
    [Fact]
    public async Task Create_persists_and_returns_category()
    {
        await using var dbContext = TestDb.Create();

        var response = await CreateCategoryHandler.ExecuteAsync(
            new CreateCategoryRequest("Electronics"),
            new CreateCategoryValidator(),
            dbContext);

        var persisted = Assert.Single(dbContext.Categories);
        Assert.Equal(persisted.Id, response.Id);
        Assert.Equal("Electronics", response.Name);
    }

    [Fact]
    public async Task Get_all_returns_projected_categories()
    {
        await using var dbContext = TestDb.Create();
        dbContext.Categories.AddRange(
            new Category { Name = "Electronics" },
            new Category { Name = "Books" });
        await dbContext.SaveChangesAsync();

        var response = await GetCategoriesHandler.ExecuteAsync(dbContext);

        Assert.Equal(2, response.Count);
        Assert.Contains(response, category => category.Name == "Electronics");
        Assert.Contains(response, category => category.Name == "Books");
    }

    [Fact]
    public async Task Get_by_id_returns_category_or_not_found()
    {
        await using var dbContext = TestDb.Create();
        var category = new Category { Name = "Electronics" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var response = await GetCategoryByIdHandler.ExecuteAsync(category.Id, dbContext);

        Assert.Equal(category.Id, response.Id);
        await Assert.ThrowsAsync<NotFoundException>(
            () => GetCategoryByIdHandler.ExecuteAsync(Guid.NewGuid(), dbContext));
    }
}
