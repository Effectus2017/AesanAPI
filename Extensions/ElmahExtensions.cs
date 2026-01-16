using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Extensions;

public static class ElmahExtensions
{
    public static async Task RaiseError(this HttpContext context, Exception exception)
    {
        if (exception != null)
        {
            try
            {
                var errorLog = context.RequestServices.GetService<ErrorLog>();
                if (errorLog != null)
                {
                    var error = new Error(exception, context);
                    await errorLog.LogAsync(error);
                    
                    // Log para diagnóstico en desarrollo
                    var logger = context.RequestServices.GetService<ILogger<ErrorLog>>();
                    logger?.LogDebug("Error registrado en ELMAH: {ExceptionType}", exception.GetType().Name);
                }
                else
                {
                    var logger = context.RequestServices.GetService<ILogger<ErrorLog>>();
                    logger?.LogWarning("ErrorLog service no está disponible. El error no se registrará en ELMAH.");
                }
            }
            catch (Exception ex)
            {
                // Si hay un error al registrar en Elmah, loguearlo pero no fallar
                var logger = context.RequestServices.GetService<ILogger<ErrorLog>>();
                logger?.LogError(ex, "Error al intentar registrar excepción en ELMAH: {Message}", ex.Message);
            }
        }
    }
}