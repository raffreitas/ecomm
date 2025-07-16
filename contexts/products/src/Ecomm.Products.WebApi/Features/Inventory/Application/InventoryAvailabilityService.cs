using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;

namespace Ecomm.Products.WebApi.Features.Inventory.Application;

public class InventoryAvailabilityService : IInventoryAvailabilityService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryAvailabilityService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> HasAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId, cancellationToken);
        if (inventory is null)
            return false;
        return inventory.IsActive && inventory.AvailableStock().Value > 0;
    }
}
