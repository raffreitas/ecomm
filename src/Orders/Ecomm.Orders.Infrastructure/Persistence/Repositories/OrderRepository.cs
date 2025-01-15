
using Ecomm.Orders.Domain.Entities;
using Ecomm.Orders.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Orders.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _dbContext;

    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var savedOrder = await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return savedOrder.Entity;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
}