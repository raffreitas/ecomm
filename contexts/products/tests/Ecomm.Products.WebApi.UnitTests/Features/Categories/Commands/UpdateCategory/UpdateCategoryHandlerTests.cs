using Ecomm.Products.WebApi.Features.Categories.Commands.UpdateCategory;
using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Shouldly;

namespace Ecomm.Products.WebApi.UnitTests.Features.Categories.Commands.UpdateCategory;

[Trait("UnitTests", "Categories - Commands")]
public sealed class UpdateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests()
    {
        _handler = new UpdateCategoryHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCategory_WhenCategoryExists()
    {
        // Arrange
        var parentCategory = Category.Create("Parent Category", null);
        var category = Category.Create("Old Category", parentCategory);

        var parentCategoryId = parentCategory.Id;
        var command = new UpdateCategoryCommand
        {
            Id = category.Id,
            Name = "Updated Category",
            ParentCategoryId = parentCategoryId
        };

        _categoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.GetByIdAsync(parentCategoryId, Arg.Any<CancellationToken>())
            .Returns(parentCategory);
        // Act
        await _handler.Handle(command, CancellationToken.None);
        // Assert
        await _categoryRepository.Received(1).UpdateAsync(category, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        category.Name.ShouldBe(command.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new UpdateCategoryCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Category"
        };
        _categoryRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Category?)null);
        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenParentCategoryDoesNotExist()
    {
        // Arrange
        var category = Category.Create("Old Category", null);
        var command = new UpdateCategoryCommand
        {
            Id = category.Id,
            Name = "Updated Category",
            ParentCategoryId = Guid.NewGuid() // Non-existing parent
        };
        _categoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, Arg.Any<CancellationToken>())
            .ReturnsNull();
        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
