using Ecomm.Products.WebApi.Features.Categories.Domain.Repositories;
using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Domain.Exceptions;

namespace Ecomm.Products.WebApi.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException($"Category {command.Id} not found.");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
