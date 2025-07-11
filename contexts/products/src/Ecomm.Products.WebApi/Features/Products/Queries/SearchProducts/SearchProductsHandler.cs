
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Inventory.Application;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

internal sealed class SearchProductsHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryAvailabilityService _inventoryAvailabilityService;
    private readonly ICategoryRepository _categoryRepository;

    public SearchProductsHandler(
        IProductRepository productRepository,
        IInventoryAvailabilityService inventoryAvailabilityService,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _inventoryAvailabilityService = inventoryAvailabilityService;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<SearchProductsResponse>> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var searchResult = await _productRepository.SearchAsync(
            query.SearchTerm,
            query.MinPrice,
            query.MaxPrice,
            query.Currency,
            query.Page,
            query.PageSize,
            ct);

        var filtered = new List<SearchProductsResponse>();
        foreach (var product in searchResult.Items)
        {
            if (!product.IsListed)
                continue;
            if (!await _inventoryAvailabilityService.HasAvailableStockAsync(product.Id, ct))
                continue;

            var categories = new List<string>();
            foreach (var categoryId in product.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, ct);
                if (category != null)
                    categories.Add(category.Name);
            }

            filtered.Add(new SearchProductsResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                IsListed = product.IsListed,
                Categories = [.. categories],
                CreatedAt = product.CreatedAt
            });
        }

        return PagedResult<SearchProductsResponse>.Create(
            filtered,
            filtered.Count,
            searchResult.Page,
            searchResult.PageSize);
    }
}
