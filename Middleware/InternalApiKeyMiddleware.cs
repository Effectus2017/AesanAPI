using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.Middleware;

/// <summary>
/// Valida el header X-Api-Key para rutas bajo /notifications (uso interno / Web Job).
/// Si la ruta no es /notifications*, no hace nada. Si es /notifications* y el API Key no coincide, devuelve 401.
/// </summary>
public class InternalApiKeyMiddleware(RequestDelegate next, IOptions<ApplicationSettings> appSettings)
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string NotificationsPathPrefix = "/notifications";
    private readonly RequestDelegate _next = next;
    private readonly string? _expectedApiKey = appSettings?.Value?.InternalApiKey;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments(NotificationsPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_expectedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { message = "Internal API Key no configurada." });
            return;
        }

        var providedKey = context.Request.Headers[ApiKeyHeaderName].FirstOrDefault();
        if (string.IsNullOrEmpty(providedKey) || !string.Equals(_expectedApiKey, providedKey.Trim(), StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "X-Api-Key inválida o faltante." });
            return;
        }

        await _next(context);
    }
}
