namespace Ecomm.Products.WebApi.Shared.Exceptions;

public class NotFoundException : Exception
{
    public const string ErrorMessage = "The requested resource was not found.";
    public NotFoundException() : base(ErrorMessage)
    {
    }
    public NotFoundException(string message) : base(message)
    {
    }
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
