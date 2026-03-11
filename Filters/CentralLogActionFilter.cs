using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Interfaces;

namespace Api.Filters;

/// <summary>
/// Filtro global que registra cada ejecución de acción en el sistema central de logging (LogApplication).
/// Permite que los controladores no necesiten ILogger ni llamadas a log.
/// </summary>
public class CentralLogActionFilter(ICentralLogService centralLogService) : IAsyncResultFilter
{
    private readonly ICentralLogService _centralLog = centralLogService ?? throw new ArgumentNullException(nameof(centralLogService));

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next();

        try
        {
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
            var action = context.RouteData.Values["action"]?.ToString() ?? "";
            var statusCode = context.HttpContext.Response.StatusCode;
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = $"{controller}.{action}";
            var level = statusCode >= 500 ? "Error" : statusCode >= 400 ? "Warning" : "Information";
            var payload = JsonSerializer.Serialize(new { controller, action, statusCode });

            await _centralLog.LogApplicationAsync(level, message, payload, userId, statusCode.ToString(), context.HttpContext.RequestAborted);
        }
        catch
        {
            // No relanzar para no alterar la respuesta al cliente si falla el log
        }
    }
}
