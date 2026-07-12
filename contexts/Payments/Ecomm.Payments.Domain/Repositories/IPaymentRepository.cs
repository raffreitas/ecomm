using Ecomm.Payments.Domain.Entities;

namespace Ecomm.Payments.Domain.Repositories;

public interface IPaymentRepository
{
    Task CreateAsync(Payment payment, CancellationToken cancellationToken = default);
}