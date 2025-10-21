using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Interfaces;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con el dashboard.
/// Proporciona endpoints para obtener métricas del dashboard AESAN.
/// </summary>
[Route("dashboard")]
public class DashboardController(ILogger<DashboardController> logger, IAesanDashboardRepository aesanDashboardRepository) : Controller
{
    private readonly ILogger<DashboardController> _logger = logger;
    private readonly IAesanDashboardRepository _aesanDashboardRepository = aesanDashboardRepository ?? throw new ArgumentNullException(nameof(aesanDashboardRepository));

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
}
