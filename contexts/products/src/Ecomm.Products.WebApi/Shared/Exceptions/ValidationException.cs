namespace Ecomm.Products.WebApi.Shared.Exceptions;

public sealed class ValidationException : Exception
{
    public const string ErrorMessage = "Error on validation exception.";
    public string[] Errors { get; }

    public ValidationException(string[] errors) : base(ErrorMessage)
    {
        Errors = errors;
    }
    public ValidationException(string error) : base(ErrorMessage)
    {
        Errors = [error];
    }
    public ValidationException(string error, Exception innerException) : base(ErrorMessage, innerException)
    {
        Errors = [error];
    }
    public ValidationException(string[] errors, Exception innerException) : base(ErrorMessage, innerException)
    {
        Errors = errors;
    }
}
