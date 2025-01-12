using Ecomm.Payments.Domain.Entities;
using Ecomm.Payments.Domain.Enums;
using Ecomm.Payments.Domain.Repositories;
using Ecomm.Payments.Domain.Services;

using MediatR;

namespace Ecomm.Payments.Application.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _paymentRepository;

    public ProcessPaymentCommandHandler(IPaymentService paymentService, IPaymentRepository paymentRepository)
    {
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
    }

    public async Task Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(request.Id, request.Total, request.CustomerDocument, request.CustomerName);

        var result = await _paymentService.ProcessPaymentAsync(payment, cancellationToken);

        if (result.Status == PaymentStatus.Approved)
            payment.MarkAsApproved();
        else if (result.Status == PaymentStatus.Rejected)
            payment.MarkAsRejected();

        await _paymentRepository.CreateAsync(payment, cancellationToken);
    }
}