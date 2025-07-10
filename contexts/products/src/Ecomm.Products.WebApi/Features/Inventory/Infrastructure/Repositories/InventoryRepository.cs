using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;

using InventoryEntity = Ecomm.Products.WebApi.Features.Inventory.Domain.Inventory;

namespace Ecomm.Products.WebApi.Features.Inventory.Infrastructure.Repositories;

public sealed class InventoryRepository(ApplicationDbContext dbContext) : IInventoryRepository
{
    public async Task AddAsync(InventoryEntity inventory, CancellationToken cancellationToken = default)
        => await dbContext.Inventories.AddAsync(inventory, cancellationToken);

    public async Task<InventoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Inventories
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<InventoryEntity?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await dbContext.Inventories
            .Where(i => i.ProductId == productId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<InventoryEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Inventories.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<InventoryEntity>.Create(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<InventoryEntity>> GetLowStockAsync(int threshold, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Inventories
            .Where(i => i.Quantity.Value <= threshold);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.Quantity.Value)
            .ThenBy(i => i.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<InventoryEntity>.Create(items, totalCount, page, pageSize);
    }

    public Task UpdateAsync(InventoryEntity inventory, CancellationToken cancellationToken = default)
    {
        dbContext.Inventories.Update(inventory);
        return Task.CompletedTask;
    }
}
