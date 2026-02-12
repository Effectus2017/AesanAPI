using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("emaillog")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los logs de correos electrónicos.
/// Proporciona endpoints para consultar el historial de envíos y reenviar correos.
/// </summary>
public class EmailLogController(IEmailLogRepository emailLogRepository, IEmailService emailService, ILoggingService loggingService) : ControllerBase
{
    private readonly IEmailLogRepository _emailLogRepository = emailLogRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly ILoggingService _loggingService = loggingService;

    /// <summary>
    /// Obtiene todos los logs de correo electrónico de un usuario específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el UserId</param>
    /// <returns>Lista de logs de correo del usuario</returns>
    [HttpGet("get-email-logs-by-user-id")]
    [SwaggerOperation(Summary = "Obtiene logs de correo por usuario", Description = "Devuelve todos los logs de correo electrónico asociados a un usuario específico.")]
    public async Task<IActionResult> GetEmailLogsByUserId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El ID de usuario es requerido." });
                }

                _loggingService.LogInformation("Obteniendo logs de correo para usuario {UserId}", new Dictionary<string, string> { { "UserId", queryParameters.UserId } });
                var logs = await _emailLogRepository.GetEmailLogsByUserId(queryParameters.UserId);
                return StatusCode(StatusCodes.Status200OK, logs);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener logs de correo por usuario", new Dictionary<string, string> { { "UserId", queryParameters?.UserId ?? "null" } });
            return StatusCode(StatusCodes.Status500InternalServerError, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene todos los logs de correo electrónico para un email específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el Email</param>
    /// <returns>Lista de logs de correo para ese email</returns>
    [HttpGet("get-email-logs-by-email")]
    [SwaggerOperation(Summary = "Obtiene logs de correo por email", Description = "Devuelve todos los logs de correo electrónico para un email específico.")]
    public async Task<IActionResult> GetEmailLogsByEmail([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.Email))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El email es requerido." });
                }

                _loggingService.LogInformation("Obteniendo logs de correo para email {Email}", new Dictionary<string, string> { { "Email", queryParameters.Email } });
                var logs = await _emailLogRepository.GetEmailLogsByEmail(queryParameters.Email);
                return StatusCode(StatusCodes.Status200OK, logs);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener logs de correo por email", new Dictionary<string, string> { { "Email", queryParameters?.Email ?? "null" } });
            return StatusCode(StatusCodes.Status500InternalServerError, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene un log de correo electrónico específico por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Log de correo específico</returns>
    [HttpGet("get-email-log-by-id")]
    [SwaggerOperation(Summary = "Obtiene un log de correo por ID", Description = "Devuelve un log de correo electrónico específico por su ID.")]
    public async Task<IActionResult> GetEmailLogById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.Id <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El ID debe ser mayor que 0." });
                }

                _loggingService.LogInformation("Obteniendo log de correo con ID {Id}", new Dictionary<string, string> { { "Id", queryParameters.Id.ToString() } });
                var log = await _emailLogRepository.GetEmailLogById(queryParameters.Id);
                
                if (log == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { Message = "No se encontró el log de correo con el ID especificado." });
                }

                return StatusCode(StatusCodes.Status200OK, log);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener log de correo por ID", new Dictionary<string, string> { { "Id", queryParameters.Id.ToString() } });
            return StatusCode(StatusCodes.Status500InternalServerError, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene los logs de correos fallidos para un email específico (o todos si no se especifica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el Email (opcional)</param>
    /// <returns>Lista de logs de correos fallidos</returns>
    [HttpGet("get-failed-email-logs")]
    [SwaggerOperation(Summary = "Obtiene correos fallidos", Description = "Devuelve todos los logs de correos electrónicos que fallaron al enviarse.")]
    public async Task<IActionResult> GetFailedEmailLogs([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _loggingService.LogInformation("Obteniendo logs de correos fallidos", new Dictionary<string, string> { { "Email", queryParameters.Email ?? "Todos" } });
                var logs = await _emailLogRepository.GetFailedEmailLogs(queryParameters.Email);
                return StatusCode(StatusCodes.Status200OK, logs);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener logs de correos fallidos");
            return StatusCode(StatusCodes.Status500InternalServerError, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Reenvía un correo electrónico basado en un log existente
    /// </summary>
    /// <param name="request">Solicitud de reenvío con ID del log y flag de forzar reenvío</param>
    /// <returns>Resultado del reenvío</returns>
    [HttpPost("resend")]
    [SwaggerOperation(Summary = "Reenvía un correo electrónico", Description = "Reenvía un correo electrónico basado en un log existente. Puede forzar el reenvío incluso si el correo original fue exitoso.")]
    public async Task<IActionResult> ResendEmail([FromBody] ResendEmailRequest request)
    {
        try
        {
            if (request == null || request.EmailLogId <= 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El ID del log de correo es requerido y debe ser mayor que 0." });
            }

            _loggingService.LogInformation("Reenviando correo desde log {EmailLogId}", new Dictionary<string, string> 
            { 
                { "EmailLogId", request.EmailLogId.ToString() },
                { "ForceResend", request.ForceResend.ToString() }
            });

            var result = await _emailService.ResendEmail(request.EmailLogId, request.ForceResend);
            
            if (result)
            {
                return StatusCode(StatusCodes.Status200OK, new { Message = "Correo reenviado exitosamente." });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "No se pudo reenviar el correo. Verifique que el log existe y que el correo original no fue exitoso (o use forceResend=true)." });
            }
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al reenviar correo", new Dictionary<string, string> { { "EmailLogId", request?.EmailLogId.ToString() ?? "null" } });
            return StatusCode(StatusCodes.Status500InternalServerError, Utilities.GetResponseFromException(ex));
        }
    }
}
