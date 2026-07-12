namespace Ecomm.Catalog.Features.Contracts;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    CategoryResponse Category);
