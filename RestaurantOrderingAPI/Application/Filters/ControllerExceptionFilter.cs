using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantOrderingAPI.Application.Filters;

public class NotFoundExcept(string message) : Exception(message) { }
public class BadRequestExcept(string message) : Exception(message) { }
public class UnauthorizedAccessExcept(string message) : Exception(message) { }
public class InternalServerErrorExcept(string message) : Exception(message) { }

public class ControllerExceptionFilter(ILogger<ControllerExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger<ControllerExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var statusCode = exception switch
        {
            NotFoundExcept => StatusCodes.Status404NotFound,
            BadRequestExcept => StatusCodes.Status400BadRequest,
            UnauthorizedAccessExcept => StatusCodes.Status401Unauthorized,
            InternalServerErrorExcept => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorMessage = statusCode == StatusCodes.Status500InternalServerError
            ? "Internal Server Error"
            : exception.Message;

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        }

        context.Result = new ObjectResult(new
        {
            error = errorMessage,
        })
        {
            StatusCode = statusCode
        };
    }
}
