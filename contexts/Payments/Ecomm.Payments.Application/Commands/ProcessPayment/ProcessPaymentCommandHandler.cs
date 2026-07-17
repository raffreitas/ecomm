using Ecomm.Payments.Application.Abstractions;
using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;

using MediatR;

namespace Ecomm.Payments.Application.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandHandler(
    IPaymentService paymentService,
    IPaymentRepository paymentRepository) : IRequestHandler<ProcessPaymentCommand>
{
    public async Task Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        if (await paymentRepository.ExistsForOrderAsync(request.Id, cancellationToken))
            return;

        var payment = new Payment(request.Id, request.Total, request.CustomerDocument, request.CustomerName);
        var result = await paymentService.ProcessPaymentAsync(payment, cancellationToken);

        if (result.Status == PaymentStatus.Approved)
            payment.MarkAsApproved(result.TransactionId);
        else if (result.Status == PaymentStatus.Rejected)
            payment.MarkAsRejected("Payment provider rejected the transaction.", result.TransactionId);

        await paymentRepository.CreateAsync(payment, cancellationToken);
    }
}
