using Bogus;

using Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;
using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

using NSubstitute;

using Shouldly;

namespace Ecomm.Products.WebApi.UnitTests.Features.Categories.Commands.CreateCategory;

[Trait("UnitTests", "Categories - Commands")]
public sealed class CreateCategoryHandlerTests
{
    private readonly Faker _faker = new();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests()
    {
        _handler = new CreateCategoryHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategory_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = _faker.Commerce.Categories(1)[0],
            ParentCategoryId = null
        };

        // Act
        var categoryId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        categoryId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldCreateSubcategory_WhenParentCategoryIdIsProvided()
    {
        // Arrange
        var parentCategory = Category.Create(_faker.Commerce.Categories(1)[0], null);
        _categoryRepository.GetByIdAsync(parentCategory.Id, Arg.Any<CancellationToken>())
            .Returns(parentCategory);
        var command = new CreateCategoryCommand
        {
            Name = _faker.Commerce.Categories(1)[0],
            ParentCategoryId = parentCategory.Id
        };
        // Act
        var categoryId = await _handler.Handle(command, CancellationToken.None);
        // Assert
        await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        categoryId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenParentCategoryDoesNotExist()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = _faker.Commerce.Categories(1)[0],
            ParentCategoryId = Guid.NewGuid()
        };
        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        await _categoryRepository.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
