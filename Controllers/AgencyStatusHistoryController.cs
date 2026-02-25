using System.Security.Claims;
using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador del historial de estados de agencia.
/// GET /api/agency-status-history con agencyId, from, to, page, pageSize.
/// </summary>
[ApiController]
[Route("api/agency-status-history")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AgencyStatusHistoryController(IAgencyStatusHistoryRepository repository, ILogger<AgencyStatusHistoryController> logger) : ControllerBase
{
    private readonly IAgencyStatusHistoryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ILogger<AgencyStatusHistoryController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene el historial de estados de una agencia paginado.
    /// Requiere permiso agencystatushistory.view.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Historial de estados de agencia", Description = "Devuelve el historial de cambios de estado de una agencia con paginación y filtros de fecha.")]
    public async Task<IActionResult> GetAgencyStatusHistory([FromQuery] QueryParameters queryParameters)
    {
        if (!UserHasPermission("agencystatushistory.view"))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permiso para ver el historial de estados de agencia." });
        }

        if (queryParameters.AgencyId <= 0)
        {
            return BadRequest(new { message = "agencyId es requerido y debe ser mayor que 0." });
        }

        try
        {
            var page = queryParameters.Page < 1 ? 1 : queryParameters.Page;
            var pageSize = queryParameters.PageSize < 1 ? 20 : queryParameters.PageSize > 100 ? 100 : queryParameters.PageSize;

            var (items, totalCount) = await _repository.GetAgencyStatusHistoryPagedAsync(
                queryParameters.AgencyId,
                queryParameters.CreatedAtFrom,
                queryParameters.CreatedAtTo,
                page,
                pageSize);

            return Ok(new
            {
                data = items,
                totalCount,
                page,
                pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de estados para agencia {AgencyId}", queryParameters.AgencyId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener el historial." });
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
