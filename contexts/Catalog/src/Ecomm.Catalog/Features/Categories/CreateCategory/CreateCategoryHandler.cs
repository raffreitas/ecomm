using Ecomm.Catalog.Domain.Entities;
using Ecomm.Catalog.Features.Contracts;
using Ecomm.Catalog.Infrastructure.Persistence;

using FluentValidation;

namespace Ecomm.Catalog.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler(IValidator<Request> validator, CatalogDbContext dbContext)
{
    public async Task<CategoryResponse> ExecuteAsync(
        Request request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var category = new Category { Name = request.Name };
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name);
    }
}
