using CM_Task.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CM_Task.Api.Middleware;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException e => (StatusCodes.Status400BadRequest, "Validation failed"),
            NotFoundException e => (StatusCodes.Status404NotFound, "Resource not found"),
            InsufficientStockException e => (StatusCodes.Status409Conflict, "Insufficient stock"),
            _ => (StatusCodes.Status500InternalServerError, "Server error")
        };

        logger.LogError(exception, "Handled exception: {Title}", title);

        var details = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };


        if (exception is ValidationException validationEx)
        {
            details.Extensions["errors"] = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(details, ct);

        return true;
    }
}