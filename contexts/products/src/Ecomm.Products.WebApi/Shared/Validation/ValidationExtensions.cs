using FluentValidation.Results;

namespace Ecomm.Products.WebApi.Shared.Validation;

public static class ValidationExtensions
{
    public static string[] GetErrors(this ValidationResult validationResult)
    {
        return [.. validationResult.Errors.Select(error => error.ErrorMessage)];
    }
}
