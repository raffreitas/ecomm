using Ecomm.Products.WebApi.Features.Products.Domain.Entities;
using Ecomm.Products.WebApi.Features.Products.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.ValueObjects;
using Ecomm.Products.WebApi.Shared.Exceptions;
using Ecomm.Products.WebApi.Shared.Validation;

namespace Ecomm.Products.WebApi.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
)
{
    public async Task Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var validationResult = command.Validate();
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.GetErrors());

        var product = await productRepository.GetByIdAsync(command.Id, ct);

        if (product is null)
            throw new NotFoundException($"Product with ID {command.Id} not found.");

        var price = Price.Create(command.Price, command.Currency);
        var categories = command.Categories.Select(Category.Create).ToArray();

        product.Update(
            command.Name,
            command.Description,
            price,
            categories);

        await unitOfWork.CommitAsync(ct);
    }
}
