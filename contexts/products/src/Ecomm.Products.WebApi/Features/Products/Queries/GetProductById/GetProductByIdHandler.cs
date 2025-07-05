using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Exceptions;
using Ecomm.Products.WebApi.Shared.Validation;

namespace Ecomm.Products.WebApi.Features.Products.Queries.GetProductById;

internal sealed class GetProductByIdHandler(IProductRepository productRepository)
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var validationResult = query.Validate();
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.GetErrors());

        var product = await productRepository.GetByIdAsync(query.Id, ct);

        if (product is null)
            throw new NotFoundException($"Product with ID {query.Id} not found.");

        return new GetProductByIdResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            IsListed = product.IsListed,
            Categories = [.. product.Categories.Select(c => c.Name)],
            Images = [.. product.Images.Select(i => new ImageResponse
            {
                Url = i.Url,
                AltText = i.AltText
            })],
            CreatedAt = product.CreatedAt
        };
    }
}
