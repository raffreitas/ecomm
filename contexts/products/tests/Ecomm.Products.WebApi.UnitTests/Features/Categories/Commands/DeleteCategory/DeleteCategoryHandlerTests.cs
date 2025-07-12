using Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;
using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Ecomm.Products.WebApi.UnitTests.Features.Categories.Commands.DeleteCategory;

[Trait("UnitTests", "Categories - Commands")]
public sealed class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCategoryHandler _handler;

    public DeleteCategoryHandlerTests()
    {
        _handler = new DeleteCategoryHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        var category = Category.Create("Test Category");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).DeleteAsync(category, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
