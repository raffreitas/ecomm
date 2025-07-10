namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;

public sealed class ActivateInventoryCommand
{
    public required Guid ProductId { get; init; }
}
