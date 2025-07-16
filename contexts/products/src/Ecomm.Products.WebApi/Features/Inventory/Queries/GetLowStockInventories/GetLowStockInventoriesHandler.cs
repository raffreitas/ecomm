using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetLowStockInventories;

internal sealed class GetLowStockInventoriesHandler(IInventoryRepository inventoryRepository)
{
    public async Task<PagedResult<LowStockInventoryResponse>> Handle(GetLowStockInventoriesQuery query, CancellationToken ct)
    {
        var pagedInventories = await inventoryRepository.GetLowStockAsync(
            query.Threshold, 
            query.Page, 
            query.PageSize, 
            ct);
        
        var inventoryResponses = pagedInventories.Items.Select(inventory => new LowStockInventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity.Value,
            IsAvailable = inventory.IsAvailable,
            CreatedAt = inventory.CreatedAt
        }).ToList();

        return PagedResult<LowStockInventoryResponse>.Create(
            inventoryResponses,
            pagedInventories.TotalCount,
            pagedInventories.Page,
            pagedInventories.PageSize);
    }
}
