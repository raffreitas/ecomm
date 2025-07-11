using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? parent = null;
        if (command.ParentCategoryId.HasValue)
        {
            parent = await categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);
            if (parent is null)
                throw new NotFoundException($"Parent category {command.ParentCategoryId} not found.");
        }

        var category = Category.Create(command.Name, parent);
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return category.Id;
    }
}
