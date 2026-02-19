using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las excursiones de sitios.
/// Proporciona endpoints para la gestión completa de excursiones, incluyendo creación,
/// lectura, actualización y eliminación de registros de excursiones.
/// </summary>
[ApiController]
[Route("api/site-excursion")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SiteExcursionController : Controller
{
    private readonly ILogger<SiteExcursionController> _logger;
    private readonly ISiteExcursionRepository _siteExcursionRepository;

    public SiteExcursionController(ILogger<SiteExcursionController> logger, ISiteExcursionRepository siteExcursionRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _siteExcursionRepository = siteExcursionRepository ?? throw new ArgumentNullException(nameof(siteExcursionRepository));
    }

    /// <summary>
    /// Obtiene una excursión por su ID
    /// </summary>
    /// <param name="id">ID de la excursión</param>
    /// <returns>La excursión encontrada</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtiene una excursión por su ID", Description = "Devuelve una excursión basada en el ID proporcionado.")]
    public async Task<IActionResult> GetSiteExcursionById(int id)
    {
        try
        {
            var result = await _siteExcursionRepository.GetSiteExcursionById(id);

            if (result == null)
            {
                return NotFound($"Excursión con ID {id} no encontrada");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la excursión: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las excursiones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="includeInactive">Incluir excursiones inactivas</param>
    /// <returns>Lista de excursiones del sitio</returns>
    [HttpGet("site/{siteId}")]
    [SwaggerOperation(Summary = "Obtiene todas las excursiones de un sitio", Description = "Devuelve una lista de excursiones del sitio especificado.")]
    public async Task<IActionResult> GetSiteExcursionsBySiteId(int siteId, [FromQuery] bool includeInactive = false)
    {
        try
        {
            var result = await _siteExcursionRepository.GetSiteExcursionsBySiteId(siteId, includeInactive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las excursiones del sitio {SiteId}: {Message}", siteId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene excursiones de un sitio por rango de fechas
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <param name="includeInactive">Incluir excursiones inactivas</param>
    /// <returns>Lista de excursiones en el rango de fechas</returns>
    [HttpGet("site/{siteId}/by-date-range")]
    [SwaggerOperation(Summary = "Obtiene excursiones por rango de fechas", Description = "Devuelve una lista de excursiones del sitio en el rango de fechas especificado.")]
    public async Task<IActionResult> GetSiteExcursionsByDateRange(
        int siteId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] bool includeInactive = false)
    {
        try
        {
            var result = await _siteExcursionRepository.GetSiteExcursionsByDateRange(siteId, startDate, endDate, includeInactive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las excursiones del sitio {SiteId} por rango de fechas: {Message}", siteId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene excursiones de un sitio por grupo de niños
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroupId">ID del grupo de niños</param>
    /// <param name="includeInactive">Incluir excursiones inactivas</param>
    /// <returns>Lista de excursiones del grupo</returns>
    [HttpGet("site/{siteId}/by-group/{childGroupId}")]
    [SwaggerOperation(Summary = "Obtiene excursiones por grupo", Description = "Devuelve una lista de excursiones del sitio para el grupo especificado.")]
    public async Task<IActionResult> GetSiteExcursionsByChildGroupId(
        int siteId,
        int childGroupId,
        [FromQuery] bool includeInactive = false)
    {
        try
        {
            var result = await _siteExcursionRepository.GetSiteExcursionsByChildGroupId(siteId, childGroupId, includeInactive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las excursiones del grupo {ChildGroupId} del sitio {SiteId}: {Message}", childGroupId, siteId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Crea una nueva excursión
    /// </summary>
    /// <param name="request">Datos de la excursión a crear</param>
    /// <returns>ID de la excursión creada</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Crea una nueva excursión", Description = "Crea una nueva excursión en la base de datos.")]
    public async Task<IActionResult> CreateSiteExcursion([FromBody] SiteExcursionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _siteExcursionRepository.InsertSiteExcursion(request);
            return Ok(new { id, message = "Excursión creada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la excursión: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una excursión existente
    /// </summary>
    /// <param name="id">ID de la excursión</param>
    /// <param name="request">Datos de la excursión a actualizar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Actualiza una excursión", Description = "Actualiza una excursión existente en la base de datos.")]
    public async Task<IActionResult> UpdateSiteExcursion(int id, [FromBody] SiteExcursionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Id != id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud");
            }

            var result = await _siteExcursionRepository.UpdateSiteExcursion(request);
            if (result)
            {
                return Ok(new { message = "Excursión actualizada exitosamente" });
            }

            return NotFound($"Excursión con ID {id} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la excursión: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina una excursión (soft delete)
    /// </summary>
    /// <param name="id">ID de la excursión</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Elimina una excursión", Description = "Elimina una excursión de la base de datos (soft delete).")]
    public async Task<IActionResult> DeleteSiteExcursion(int id)
    {
        try
        {
            var result = await _siteExcursionRepository.DeleteSiteExcursion(id);
            if (result)
            {
                return Ok(new { message = "Excursión eliminada exitosamente" });
            }

            return NotFound($"Excursión con ID {id} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la excursión: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}

