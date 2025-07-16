using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Exceptions;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException($"Category {command.Id} not found.");

        await categoryRepository.DeleteAsync(category, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
