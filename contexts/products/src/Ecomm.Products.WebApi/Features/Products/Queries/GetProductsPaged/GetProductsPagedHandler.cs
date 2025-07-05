using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductsPaged;

internal sealed class GetProductsPagedHandler(IProductRepository productRepository)
{
    public async Task<PagedResult<ProductSummaryResponse>> Handle(GetProductsPagedQuery query, CancellationToken ct)
    {
        var pagedProducts = await productRepository.GetPagedAsync(query.Page, query.PageSize, ct);
        
        var productSummaries = pagedProducts.Items.Select(product => new ProductSummaryResponse
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

        return PagedResult<ProductSummaryResponse>.Create(
            productSummaries,
            pagedProducts.TotalCount,
            pagedProducts.Page,
            pagedProducts.PageSize);
    }
}
