namespace Ecomm.Products.WebApi.Features.Inventory.Commands.ActivateInventory;

public sealed record ActivateInventoryCommand
{
    public required Guid ProductId { get; init; }
}
