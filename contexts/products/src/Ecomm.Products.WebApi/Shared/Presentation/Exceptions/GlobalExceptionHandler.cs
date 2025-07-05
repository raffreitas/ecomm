
using Ecomm.Products.WebApi.Shared.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Products.WebApi.Shared.Presentation.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var problemDetails = new ProblemDetails { Instance = httpContext.Request.Path };

        if (exception is NotFoundException notFoundException)
        {
            problemDetails.Title = "Resource not found";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundException.Message;
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation error.";
            problemDetails.Detail = validationException.Message;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            if (validationException.Errors.Length > 0)
            {
                problemDetails.Extensions["errors"] = validationException.Errors;
            }
        }
        else
        {
            problemDetails.Title = "Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
