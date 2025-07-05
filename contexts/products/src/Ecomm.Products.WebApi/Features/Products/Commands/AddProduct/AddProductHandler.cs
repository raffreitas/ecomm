using Ecomm.Products.WebApi.Features.Products.Domain;
using Ecomm.Products.WebApi.Features.Products.Domain.Entities;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.ValueObjects;

namespace Ecomm.Products.WebApi.Features.Products.Commands.AddProduct;

internal sealed class AddProductHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
)
{
    public async Task Handle(AddProductCommand command, CancellationToken ct)
    {
        var product = Product.Create(
            command.Name,
            command.Description,
            Price.Create(command.Price, command.Currency),
            [.. command.Categories.Select(Category.Create)]
        );

        await productRepository.AddAsync(product, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
