using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Application.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Ecomm.Payments.Infrastructure.Persistence.Repositories;

public class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        => dbContext.Payments.AnyAsync(payment => payment.OrderId == orderId, cancellationToken);

    public async Task CreateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
