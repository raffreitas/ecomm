namespace Ecomm.Products.WebApi.Features.Inventory.Commands.DeactivateInventory;

public sealed class DeactivateInventoryCommand
{
    public required Guid ProductId { get; init; }
}
