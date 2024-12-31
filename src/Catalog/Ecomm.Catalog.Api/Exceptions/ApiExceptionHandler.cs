using Ecomm.Catalog.Api.Exceptions.Base;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Catalog.Api.Exceptions;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails { Instance = httpContext.Request.Path, };

        if (exception is NotFoundException notFoundException)
        {
            problemDetails.Title = "Resource not found";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundException.Message;
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
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