namespace Ecomm.Products.WebApi.Features.Products.Commands.ToggleProductListing;

public sealed record ToggleProductListingCommand
{
    public required Guid ProductId { get; init; }
    public required bool List { get; init; }
}
