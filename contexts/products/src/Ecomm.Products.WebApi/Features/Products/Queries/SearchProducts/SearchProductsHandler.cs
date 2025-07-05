using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Products.Queries.SearchProducts;

internal sealed class SearchProductsHandler(IProductRepository productRepository)
{
    public async Task<PagedResult<SearchProductsResponse>> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var searchResult = await productRepository.SearchAsync(
            query.SearchTerm,
            query.MinPrice,
            query.MaxPrice,
            query.Currency,
            query.Page,
            query.PageSize,
            ct);

        var responseItems = searchResult.Items.Select(product => new SearchProductsResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            IsListed = product.IsListed,
            Categories = product.Categories.Select(c => c.Name).ToArray(),
            CreatedAt = product.CreatedAt
        }).ToList();

        return PagedResult<SearchProductsResponse>.Create(
            responseItems,
            searchResult.TotalCount,
            searchResult.Page,
            searchResult.PageSize);
    }
}
