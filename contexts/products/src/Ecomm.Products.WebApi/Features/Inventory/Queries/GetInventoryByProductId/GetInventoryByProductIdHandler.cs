using Ecomm.Products.WebApi.Features.Inventory.Domain.Repositories;

namespace Ecomm.Products.WebApi.Features.Inventory.Queries.GetInventoryByProductId;

internal sealed class GetInventoryByProductIdHandler(IInventoryRepository inventoryRepository)
{
    public async Task<GetInventoryByProductIdResponse?> Handle(GetInventoryByProductIdQuery query, CancellationToken ct)
    {
        var inventory = await inventoryRepository.GetByProductIdAsync(query.ProductId, ct);
        
        if (inventory is null)
            return null;

        return new GetInventoryByProductIdResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity.Value,
            IsAvailable = inventory.IsAvailable,
            CreatedAt = inventory.CreatedAt
        };
    }
}
