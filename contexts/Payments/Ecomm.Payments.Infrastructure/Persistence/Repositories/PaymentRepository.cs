using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Repositories;

namespace Ecomm.Payments.Infrastructure.Persistence.Repositories;

public class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public async Task CreateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}