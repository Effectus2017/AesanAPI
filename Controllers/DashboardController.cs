using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con el dashboard.
/// Proporciona endpoints para obtener métricas del dashboard AESAN y de agencia.
/// </summary>
[ApiController]
[Route("dashboard")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController(ILogger<DashboardController> logger, IAesanDashboardRepository aesanDashboardRepository, IAgencyDashboardRepository agencyDashboardRepository) : Controller
{
    private readonly ILogger<DashboardController> _logger = logger;
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
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo métricas del dashboard AESAN para usuario: {UserId}", queryParameters.UserId);

                var metrics = await _aesanDashboardRepository.GetDashboardMetrics(queryParameters.UserId);

                return Ok(metrics);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas del dashboard AESAN: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
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
        try
        {
            // Verificar que el usuario esté autenticado
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("Usuario no autenticado al intentar obtener métricas del dashboard de agencia");
                return Unauthorized("Usuario no autenticado");
            }

            // Log todos los claims para debugging
            _logger.LogInformation("Claims del usuario: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

            // Obtener agencyId del token del usuario autenticado
            var agencyIdClaim = User.FindFirst("agencyId")?.Value;

            if (string.IsNullOrEmpty(agencyIdClaim))
            {
                _logger.LogWarning("No se encontró el claim 'agencyId' en el token del usuario. Claims disponibles: {Claims}",
                    string.Join(", ", User.Claims.Select(c => c.Type)));
                return Unauthorized("No se pudo obtener el ID de la agencia del usuario autenticado");
            }

            if (!int.TryParse(agencyIdClaim, out var agencyId))
            {
                _logger.LogWarning("El agencyId '{AgencyId}' no es un número válido", agencyIdClaim);
                return Unauthorized("El ID de la agencia no es válido");
            }

            _logger.LogInformation("Obteniendo métricas del dashboard de agencia para agencyId: {AgencyId}", agencyId);

            var metrics = await _agencyDashboardRepository.GetDashboardMetrics(agencyId);

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas del dashboard de agencia: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
