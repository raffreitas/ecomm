using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoriesPaged;

internal sealed class GetInventoriesPagedHandler(IInventoryRepository inventoryRepository)
{
    public async Task<PagedResult<InventorySummaryResponse>> Handle(GetInventoriesPagedQuery query, CancellationToken ct)
    {
        var pagedInventories = await inventoryRepository.GetPagedAsync(query.Page, query.PageSize, ct);
        
        var inventorySummaries = pagedInventories.Items.Select(inventory => new InventorySummaryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity.Value,
            IsAvailable = inventory.IsAvailable,
            CreatedAt = inventory.CreatedAt
        }).ToList();

        return PagedResult<InventorySummaryResponse>.Create(
            inventorySummaries,
            pagedInventories.TotalCount,
            pagedInventories.Page,
            pagedInventories.PageSize);
    }
}
