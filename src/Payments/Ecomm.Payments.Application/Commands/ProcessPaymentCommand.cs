using MediatR;

namespace Ecomm.Payments.Application.Commands;

public record ProcessPaymentCommand(
    Guid Id,
    string CustomerName,
    string CustomerDocument,
    string CardHash,
    decimal Total) : IRequest;