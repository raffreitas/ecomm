namespace Ecomm.Products.WebApi.Features.Inventory.Application;

public interface IInventoryAvailabilityService
{
    Task<bool> HasAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default);
}
