using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con el dashboard.
/// Proporciona endpoints para obtener métricas del dashboard AESAN y de agencia.
/// </summary>
[ApiController]
[Route("dashboard")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class DashboardController(IAesanDashboardRepository aesanDashboardRepository, IAgencyDashboardRepository agencyDashboardRepository) : Controller
{
    private readonly IAesanDashboardRepository _aesanDashboardRepository = aesanDashboardRepository ?? throw new ArgumentNullException(nameof(aesanDashboardRepository));
    private readonly IAgencyDashboardRepository _agencyDashboardRepository = agencyDashboardRepository ?? throw new ArgumentNullException(nameof(agencyDashboardRepository));

    /// <summary>
    /// Obtiene las métricas del dashboard AESAN
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del usuario</param>
    /// <returns>Las métricas del dashboard</returns>
    [HttpGet("aesan-metrics")]
    [SwaggerOperation(Summary = "Obtiene las métricas del dashboard AESAN", Description = "Devuelve las métricas del dashboard incluyendo conteos de agencias por estado.")]
    public async Task<IActionResult> GetAesanMetrics([FromQuery] QueryParameters queryParameters)
    {
        var metrics = await _aesanDashboardRepository.GetDashboardMetrics(queryParameters.UserId);

        return Ok(metrics);
    }

    /// <summary>
    /// Obtiene las métricas del dashboard de agencia
    /// </summary>
    /// <returns>Las métricas del dashboard de agencia</returns>
    [HttpGet("agency-metrics")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Obtiene las métricas del dashboard de agencia", Description = "Devuelve las métricas del dashboard incluyendo conteos de sitios y escuelas de la agencia del usuario autenticado.")]
    public async Task<IActionResult> GetAgencyMetrics()
    {
        // Verificar que el usuario esté autenticado
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized("Usuario no autenticado");
        }

        // Obtener agencyId del token del usuario autenticado
        var agencyIdClaim = User.FindFirst("agencyId")?.Value;

        if (string.IsNullOrEmpty(agencyIdClaim))
        {
            return Unauthorized("No se pudo obtener el ID de la agencia del usuario autenticado");
        }

        if (!int.TryParse(agencyIdClaim, out var agencyId))
        {
            return Unauthorized("El ID de la agencia no es válido");
        }

        var metrics = await _agencyDashboardRepository.GetDashboardMetrics(agencyId);

        return Ok(metrics);
    }
}
