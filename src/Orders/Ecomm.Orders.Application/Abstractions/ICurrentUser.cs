namespace Ecomm.Orders.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
}