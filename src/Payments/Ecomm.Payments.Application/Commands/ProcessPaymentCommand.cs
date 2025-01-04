using MediatR;

namespace Ecomm.Payments.Application.Commands;

public record ProcessPaymentCommand(Guid Id, string CardHash, decimal Total) : IRequest;