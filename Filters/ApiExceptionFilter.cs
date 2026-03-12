using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Models.Errors;
using System.Net;

namespace Api.Filters;

/// <summary>
/// Filtro global para manejar excepciones y devolver una respuesta estandarizada
/// </summary>
public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        ApiException apiException;

        if (exception is ApiException ex)
        {
            apiException = ex;
            _logger.LogWarning("API Exception: {Code} - {Message}", apiException.Code, apiException.Message);
        }
        else
        {
            apiException = new ApiException(ErrorCode.UNEXPECTED_ERROR, exception.Message, (int)HttpStatusCode.InternalServerError);
            _logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);
        }

        context.Result = new ObjectResult(new { code = apiException.Code, message = apiException.Message })
        {
            StatusCode = apiException.StatusCode
        };

        context.ExceptionHandled = true;
    }
}
