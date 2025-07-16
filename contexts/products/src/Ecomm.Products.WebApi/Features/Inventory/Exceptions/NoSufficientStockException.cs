namespace Ecomm.Products.WebApi.Features.Inventory.Exceptions;

public sealed class NoSufficientStockException : Exception
{
    public NoSufficientStockException(Guid productId, int requestedQuantity)
        : base($"Insufficient stock for Product with ID '{productId}'. Requested quantity: {requestedQuantity}.")
    {
    }
}
