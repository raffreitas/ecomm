using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;

public interface IInventoryRepository
{
    Task AddAsync(InventoryEntity inventory, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryEntity inventory, CancellationToken cancellationToken = default);
    Task<InventoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryEntity?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<InventoryEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<InventoryEntity>> GetLowStockAsync(int threshold, int page, int pageSize, CancellationToken cancellationToken = default);
}
