
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Domain.Pagination;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductsPaged;

internal sealed class GetProductsPagedHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
{
    public async Task<PagedResult<ProductSummaryResponse>> Handle(GetProductsPagedQuery query, CancellationToken ct)
    {
        var pagedProducts = await productRepository.GetPagedAsync(query.Page, query.PageSize, ct);

        var productSummaries = new List<ProductSummaryResponse>();
        foreach (var product in pagedProducts.Items)
        {
            var categories = new List<string>();
            foreach (var categoryId in product.CategoryIds)
            {
                var category = await categoryRepository.GetByIdAsync(categoryId, ct);
                if (category != null)
                    categories.Add(category.Name);
            }
            productSummaries.Add(new ProductSummaryResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                IsListed = product.IsListed,
                Categories = categories.ToArray(),
                CreatedAt = product.CreatedAt
            });
        }

        return PagedResult<ProductSummaryResponse>.Create(
            productSummaries,
            pagedProducts.TotalCount,
            pagedProducts.Page,
            pagedProducts.PageSize);
    }
}
