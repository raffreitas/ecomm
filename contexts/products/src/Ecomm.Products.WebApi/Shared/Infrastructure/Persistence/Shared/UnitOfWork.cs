using Ecomm.Products.WebApi.Shared.Abstractions;
using Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Context;

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Shared;

internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
