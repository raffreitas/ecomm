using Ecomm.Catalog.Api.Models;
using Ecomm.Catalog.Api.Models.InputModel;
using Ecomm.Catalog.Api.Models.ViewModels;

namespace Ecomm.Catalog.Api.Services.Contracts;

public interface IProductService
{
    Task<IList<ProductViewModel>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductViewModel?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IList<ProductViewModel>> GetProductByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task CreateProductAsync(CreateProductInputModel product, CancellationToken cancellationToken = default);
}