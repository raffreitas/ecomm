using Bogus;

using Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;
using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Domain.ValueObject;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Shouldly;

using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.UnitTests.Features.Inventory.Commands.ActivateInventory;

[Trait("UnitTests", "Inventories - Commands")]
public sealed class ActivateInventoryHandlerTests
{
    private readonly Faker _faker = new();
    private readonly IInventoryRepository _inventoryRepository = Substitute.For<IInventoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly ActivateInventoryHandler _handler;

    public ActivateInventoryHandlerTests()
    {
        _handler = new ActivateInventoryHandler(_inventoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateInventory_WhenCommandIsValid()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var inventory = InventoryEntity.Create(productId, Quantity.Create(1));
        _inventoryRepository.GetByProductIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(inventory);
        var command = new ActivateInventoryCommand { ProductId = productId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _inventoryRepository.Received(1).UpdateAsync(inventory, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenInventoryDoesNotExist()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        _inventoryRepository.GetByProductIdAsync(productId, Arg.Any<CancellationToken>())
            .ReturnsNull();
        var command = new ActivateInventoryCommand { ProductId = productId };

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldActivateInventory_WhenInventoryIsInactive()
    {
        // Arrange
        var productId = _faker.Random.Guid();
        var inventory = InventoryEntity.Create(productId, Quantity.Create(1));
        inventory.Deactivate();
        _inventoryRepository.GetByProductIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(inventory);
        var command = new ActivateInventoryCommand { ProductId = productId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _inventoryRepository.Received(1).UpdateAsync(inventory, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
