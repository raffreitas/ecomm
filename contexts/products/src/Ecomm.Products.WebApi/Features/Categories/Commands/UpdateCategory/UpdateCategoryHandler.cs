using Ecomm.Products.WebApi.Features.Categories.Domain;
using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException($"Category {command.Id} not found.");

        Category? parent = null;
        if (command.ParentCategoryId.HasValue)
        {
            parent = await _categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);
            if (parent is null)
                throw new NotFoundException($"Parent category {command.ParentCategoryId} not found.");
        }

        category.Update(command.Name, parent);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
