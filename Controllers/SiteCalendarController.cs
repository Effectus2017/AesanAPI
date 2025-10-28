using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Dapper;
using Api.Data;
using Api.Interfaces;
using System.Data;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar el calendario de funcionamiento de sitios
/// Proporciona endpoints para la gestión completa de días de funcionamiento,
/// incluyendo consulta, creación, actualización y eliminación de días específicos.
/// </summary>
[Route("site-calendar")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SiteCalendarController(ILogger<SiteCalendarController> logger, ISiteCalendarRepository siteCalendarRepository, DapperContext context) : Controller
{
    private readonly ILogger<SiteCalendarController> _logger = logger;
    private readonly ISiteCalendarRepository _siteCalendarRepository = siteCalendarRepository ?? throw new ArgumentNullException(nameof(siteCalendarRepository));
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Obtiene todos los días de funcionamiento de un sitio específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del sitio</param>
    /// <returns>Información del sitio y sus días de funcionamiento</returns>
    [HttpGet("get-operating-days")]
    [SwaggerOperation(Summary = "Obtiene días de funcionamiento de un sitio", Description = "Devuelve todos los días de funcionamiento para un sitio específico.")]
    public async Task<IActionResult> GetOperatingDays([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.SiteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            _logger.LogInformation("Obteniendo días de funcionamiento para el sitio {SiteId}", queryParameters.SiteId);

            // Verificar que el sitio existe
            var siteExists = await _siteCalendarRepository.SiteExists(queryParameters.SiteId.Value);
            if (!siteExists)
            {
                _logger.LogWarning("Sitio {SiteId} no encontrado", queryParameters.SiteId);
                return NotFound($"Sitio con ID {queryParameters.SiteId} no encontrado");
            }

            var result = await _siteCalendarRepository.GetOperatingDays(queryParameters.SiteId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días de funcionamiento para el sitio {SiteId}", queryParameters.SiteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Alterna el estado de funcionamiento de un día específico
    /// </summary>
    /// <param name="request">Datos del día de funcionamiento</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("toggle-operating-day")]
    [SwaggerOperation(Summary = "Alterna estado de funcionamiento de un día", Description = "Inserta o actualiza el estado de funcionamiento de un día específico.")]
    public async Task<IActionResult> ToggleOperatingDay([FromBody] SiteOperatingDayRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Alternando día de funcionamiento para sitio {SiteId} en fecha {Date}",
                request.SiteId, request.OperatingDate.Date);

            // Verificar que el sitio existe
            var siteExists = await _siteCalendarRepository.SiteExists(request.SiteId);
            if (!siteExists)
            {
                _logger.LogWarning("Sitio {SiteId} no encontrado", request.SiteId);
                return NotFound($"Sitio con ID {request.SiteId} no encontrado");
            }

            bool result;

            // Si tiene ID, actualizar el registro existente
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                _logger.LogInformation("Actualizando día de funcionamiento {Id} para sitio {SiteId} en fecha {Date}",
                    request.Id.Value, request.SiteId, request.OperatingDate.Date);
                result = await _siteCalendarRepository.UpdateOperatingDay(request.Id.Value, request);
            }
            else
            {
                _logger.LogInformation("Alternando día de funcionamiento para sitio {SiteId} en fecha {Date}",
                    request.SiteId, request.OperatingDate.Date);
                result = await _siteCalendarRepository.ToggleOperatingDay(request);
            }

            if (result)
            {
                _logger.LogInformation("Día de funcionamiento alternado exitosamente para sitio {SiteId} en fecha {Date}",
                    request.SiteId, request.OperatingDate.Date);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudo alternar el día de funcionamiento para sitio {SiteId} en fecha {Date}",
                    request.SiteId, request.OperatingDate.Date);
                return BadRequest("No se pudo procesar la solicitud");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al alternar día de funcionamiento para sitio {SiteId} en fecha {Date}",
                request.SiteId, request.OperatingDate.Date);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza múltiples días de funcionamiento en lote
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="requests">Lista de días de funcionamiento a actualizar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("bulk-update-operating-days")]
    [SwaggerOperation(Summary = "Actualiza múltiples días de funcionamiento", Description = "Actualiza múltiples días de funcionamiento en una sola operación.")]
    public async Task<IActionResult> BulkUpdateOperatingDays([FromQuery] int siteId, [FromBody] List<SiteOperatingDayRequest> requests)
    {
        try
        {
            if (siteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            if (requests == null || !requests.Any())
            {
                return BadRequest("La lista de días de funcionamiento no puede estar vacía");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Actualizando {Count} días de funcionamiento para sitio {SiteId}",
                requests.Count, siteId);

            // Verificar que el sitio existe
            var siteExists = await _siteCalendarRepository.SiteExists(siteId);
            if (!siteExists)
            {
                _logger.LogWarning("Sitio {SiteId} no encontrado", siteId);
                return NotFound($"Sitio con ID {siteId} no encontrado");
            }

            // Validar que todos los requests pertenecen al mismo sitio
            var invalidRequests = requests.Where(r => r.SiteId != siteId).ToList();
            if (invalidRequests.Any())
            {
                return BadRequest("Todos los días de funcionamiento deben pertenecer al mismo sitio");
            }

            var result = await _siteCalendarRepository.BulkUpdateOperatingDays(siteId, requests);

            if (result)
            {
                _logger.LogInformation("Actualización en lote exitosa para sitio {SiteId}", siteId);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("Actualización en lote fallida para sitio {SiteId}", siteId);
                return BadRequest("No se pudieron procesar todas las solicitudes");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización en lote para sitio {SiteId}", siteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina un día de funcionamiento
    /// </summary>
    /// <param name="id">ID del día de funcionamiento</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("delete-operating-day/{id}")]
    [SwaggerOperation(Summary = "Elimina un día de funcionamiento", Description = "Elimina un día de funcionamiento específico.")]
    public async Task<IActionResult> DeleteOperatingDay(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser mayor a 0");
            }

            _logger.LogInformation("Eliminando día de funcionamiento {Id}", id);

            // Verificar que el día existe consultando directamente
            using var dbConnection = _context.CreateConnection();
            var existsQuery = "SELECT COUNT(1) FROM SiteOperatingDays WHERE Id = @Id";
            var existsParameters = new DynamicParameters();
            existsParameters.Add("@Id", id, DbType.Int32);

            var exists = await dbConnection.QuerySingleAsync<int>(existsQuery, existsParameters);
            if (exists == 0)
            {
                _logger.LogWarning("Día de funcionamiento {Id} no encontrado", id);
                return NotFound($"Día de funcionamiento con ID {id} no encontrado");
            }

            var result = await _siteCalendarRepository.DeleteOperatingDay(id);

            if (result)
            {
                _logger.LogInformation("Día de funcionamiento {Id} eliminado exitosamente", id);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudo eliminar el día de funcionamiento {Id}", id);
                return BadRequest("No se pudo eliminar el día de funcionamiento");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar día de funcionamiento {Id}", id);
            return StatusCode(500, ex.Message);
        }
    }
}
