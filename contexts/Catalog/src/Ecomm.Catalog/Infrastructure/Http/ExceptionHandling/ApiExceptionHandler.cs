using Ecomm.Catalog.Common.Exceptions;

using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Catalog.Infrastructure.Http.ExceptionHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

            problemDetails = new HttpValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path,
            };
        }
        else if (exception is NotFoundException notFoundException)
        {
            problemDetails = new ProblemDetails
            {
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = notFoundException.Message,
                Instance = httpContext.Request.Path,
            };
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Title = "Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path,
            };
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
