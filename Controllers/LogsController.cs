using System.Security.Claims;
using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador unificado del centro de logs.
/// GET /api/logs con filtro por categoría (Audit, Email, Job, Application) y permisos por categoría.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LogsController(ILogsQueryService logsQueryService, ILogger<LogsController> logger) : ControllerBase
{
    private static readonly Dictionary<string, string> CategoryToPermission = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Audit", "log.view.audit" },
        { "Email", "log.view.email" },
        { "Job", "log.view.job" },
        { "Application", "log.view.errors" }
    };

    /// <summary>
    /// Obtiene entradas de log paginadas por categoría (Audit, Email, Job, Application).
    /// Requiere permiso según categoría: log.view.audit, log.view.email, log.view.job, log.view.errors.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Listado paginado del centro de logs", Description = "Devuelve entradas de log por categoría con paginación y filtros de fecha.")]
    public async Task<IActionResult> GetLogs([FromQuery] QueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        var category = queryParameters.LogCategory?.Trim();

        if (string.IsNullOrEmpty(category))
        {
            return BadRequest(new { message = "El parámetro category (LogCategory) es requerido. Valores: Audit, Email, Job, Application." });
        }

        if (!CategoryToPermission.TryGetValue(category, out var requiredPermission))
        {
            return BadRequest(new { message = "Categoría no válida. Valores: Audit, Email, Job, Application." });
        }

        if (!UserHasPermission(requiredPermission))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = $"No tiene permiso para ver logs de la categoría {category}." });
        }

        try
        {
            var page = queryParameters.Page < 1 ? 1 : queryParameters.Page;
            var pageSize = queryParameters.PageSize < 1 ? 20 : queryParameters.PageSize > 100 ? 100 : queryParameters.PageSize;

            var (items, totalCount) = await logsQueryService.GetLogsPagedAsync(
                category,
                queryParameters.LogFrom,
                queryParameters.LogTo,
                page,
                pageSize,
                cancellationToken);

            return Ok(new
            {
                items,
                totalCount,
                page,
                pageSize
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener logs para categoría {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener los logs." });
        }
    }

    private bool UserHasPermission(string permissionValueKey)
    {
        var permissionsClaim = User.FindFirst(c => c.Type == "permissions" && c.Value == permissionValueKey);
        if (permissionsClaim != null)
            return true;

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.Equals(role, "Super-Administrator", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
