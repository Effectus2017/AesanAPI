using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar las instalaciones de sitios
/// Proporciona endpoints para la gestión completa de instalaciones específicas de sitios,
/// incluyendo consulta, creación, actualización y eliminación.
/// </summary>
[ApiController]
[Route("site-facility")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SiteFacilityController(ILogger<SiteFacilityController> logger, ISiteFacilityRepository siteFacilityRepository) : Controller
{
    private readonly ILogger<SiteFacilityController> _logger = logger;
    private readonly ISiteFacilityRepository _siteFacilityRepository = siteFacilityRepository ?? throw new ArgumentNullException(nameof(siteFacilityRepository));

    /// <summary>
    /// Obtiene todas las instalaciones de un sitio específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del sitio</param>
    /// <returns>Lista de instalaciones del sitio</returns>
    [HttpGet("get-facilities-by-site")]
    [SwaggerOperation(Summary = "Obtiene instalaciones de un sitio", Description = "Devuelve todas las instalaciones para un sitio específico.")]
    public async Task<IActionResult> GetFacilitiesBySite([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.SiteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            _logger.LogInformation("Obteniendo instalaciones para el sitio {SiteId}", queryParameters.SiteId);

            var result = await _siteFacilityRepository.GetFacilitiesBySite(queryParameters.SiteId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener instalaciones para el sitio {SiteId}", queryParameters.SiteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="facilityTypeIds">Lista de IDs de tipos de instalación</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("update-site-facilities")]
    [SwaggerOperation(Summary = "Actualiza instalaciones de un sitio", Description = "Actualiza las instalaciones de un sitio específico.")]
    public async Task<IActionResult> UpdateSiteFacilities([FromQuery] int siteId, [FromBody] List<int> facilityTypeIds)
    {
        try
        {
            if (siteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            if (facilityTypeIds == null || !facilityTypeIds.Any())
            {
                return BadRequest("La lista de tipos de instalación no puede estar vacía");
            }

            _logger.LogInformation("Actualizando instalaciones para el sitio {SiteId}", siteId);

            var result = await _siteFacilityRepository.UpdateSiteFacilities(siteId, facilityTypeIds);

            if (result)
            {
                _logger.LogInformation("Instalaciones actualizadas exitosamente para el sitio {SiteId}", siteId);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudieron actualizar las instalaciones para el sitio {SiteId}", siteId);
                return BadRequest("No se pudieron actualizar las instalaciones");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar instalaciones para el sitio {SiteId}", siteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo de las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("update-facility-status")]
    [SwaggerOperation(Summary = "Actualiza estado de instalaciones", Description = "Actualiza el estado activo de las instalaciones de un sitio.")]
    public async Task<IActionResult> UpdateFacilityStatus([FromQuery] int siteId, [FromQuery] bool isActive)
    {
        try
        {
            if (siteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            _logger.LogInformation("Actualizando estado de instalaciones para el sitio {SiteId}: {IsActive}", siteId, isActive);

            var result = await _siteFacilityRepository.UpdateSiteFacilityIsActive(siteId, isActive);

            if (result)
            {
                _logger.LogInformation("Estado de instalaciones actualizado exitosamente para el sitio {SiteId}", siteId);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudo actualizar el estado de instalaciones para el sitio {SiteId}", siteId);
                return BadRequest("No se pudo actualizar el estado de instalaciones");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de instalaciones para el sitio {SiteId}", siteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina todas las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("delete-site-facilities")]
    [SwaggerOperation(Summary = "Elimina instalaciones de un sitio", Description = "Elimina todas las instalaciones de un sitio específico.")]
    public async Task<IActionResult> DeleteSiteFacilities([FromQuery] int siteId)
    {
        try
        {
            if (siteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            _logger.LogInformation("Eliminando instalaciones para el sitio {SiteId}", siteId);

            var result = await _siteFacilityRepository.DeleteSiteFacilities(siteId);

            if (result)
            {
                _logger.LogInformation("Instalaciones eliminadas exitosamente para el sitio {SiteId}", siteId);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudieron eliminar las instalaciones para el sitio {SiteId}", siteId);
                return BadRequest("No se pudieron eliminar las instalaciones");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar instalaciones para el sitio {SiteId}", siteId);
            return StatusCode(500, ex.Message);
        }
    }
}
