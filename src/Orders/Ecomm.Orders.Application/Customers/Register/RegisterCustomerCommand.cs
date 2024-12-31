using MediatR;

namespace Ecomm.Orders.Application.Customers.Register;

public record RegisterCustomerCommand(string Name, string Email, string Password) : IRequest;