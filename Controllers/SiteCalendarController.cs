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
/// Controlador para gestionar el calendario de funcionamiento de sitios
/// Proporciona endpoints para la gestión completa de días de funcionamiento,
/// incluyendo consulta, creación, actualización y eliminación de días específicos.
/// </summary>
[ApiController]
[Route("site-calendar")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SiteCalendarController(ILogger<SiteCalendarController> logger, ISiteCalendarRepository siteCalendarRepository) : Controller
{
    private readonly ILogger<SiteCalendarController> _logger = logger;
    private readonly ISiteCalendarRepository _siteCalendarRepository = siteCalendarRepository ?? throw new ArgumentNullException(nameof(siteCalendarRepository));

    /// <summary>
    /// Obtiene días de funcionamiento de un sitio específico
    /// Opcionalmente filtra por mes y año para mejorar el rendimiento
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del sitio, mes y año opcionales</param>
    /// <returns>Información del sitio y sus días de funcionamiento</returns>
    [HttpGet("get-operating-days")]
    [SwaggerOperation(Summary = "Obtiene días de funcionamiento de un sitio", Description = "Devuelve días de funcionamiento para un sitio específico. Opcionalmente filtra por mes y año para mejorar el rendimiento.")]
    public async Task<IActionResult> GetOperatingDays([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.SiteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor a 0");
            }

            // Validar mes si se proporciona
            if (queryParameters.Month.HasValue && (queryParameters.Month < 1 || queryParameters.Month > 12))
            {
                return BadRequest("El mes debe estar entre 1 y 12");
            }

            // Validar año si se proporciona
            if (queryParameters.Year.HasValue && queryParameters.Year < 2000)
            {
                return BadRequest("El año debe ser mayor a 2000");
            }

            _logger.LogInformation("Obteniendo días de funcionamiento para el sitio {SiteId} (mes: {Month}, año: {Year})",
                queryParameters.SiteId, queryParameters.Month?.ToString() ?? "todos", queryParameters.Year?.ToString() ?? "todos");

            var result = await _siteCalendarRepository.GetOperatingDays(
                queryParameters.SiteId.Value,
                queryParameters.Month,
                queryParameters.Year
            );
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días de funcionamiento para el sitio {SiteId}", queryParameters.SiteId);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Crea un nuevo día de funcionamiento
    /// </summary>
    /// <param name="request">Datos del día de funcionamiento</param>
    /// <returns>ID del día creado</returns>
    [HttpPost("create-operating-day")]
    [SwaggerOperation(Summary = "Crea un nuevo día de funcionamiento", Description = "Crea un nuevo día de funcionamiento para un sitio específico.")]
    public async Task<IActionResult> CreateOperatingDay([FromBody] SiteOperatingDayRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creando día de funcionamiento para sitio {SiteId} en fecha {Date}",
                request.SiteId, request.OperatingDate.Date);

            var newId = await _siteCalendarRepository.CreateOperatingDay(request);

            if (newId.HasValue)
            {
                _logger.LogInformation("Día de funcionamiento creado exitosamente con ID {Id} para sitio {SiteId} en fecha {Date}",
                    newId.Value, request.SiteId, request.OperatingDate.Date);
                return Ok(new { id = newId.Value, success = true });
            }
            else
            {
                _logger.LogWarning("No se pudo crear el día de funcionamiento para sitio {SiteId} en fecha {Date}",
                    request.SiteId, request.OperatingDate.Date);
                return BadRequest("No se pudo procesar la solicitud");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear día de funcionamiento para sitio {SiteId} en fecha {Date}",
                request.SiteId, request.OperatingDate.Date);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un día de funcionamiento existente
    /// </summary>
    /// <param name="request">Datos actualizados del día de funcionamiento</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("update-operating-day")]
    [SwaggerOperation(Summary = "Actualiza un día de funcionamiento", Description = "Actualiza un día de funcionamiento existente.")]
    public async Task<IActionResult> UpdateOperatingDay([FromBody] SiteOperatingDayRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!request.Id.HasValue || request.Id.Value <= 0)
            {
                return BadRequest("El ID del día de funcionamiento es requerido para actualizar");
            }

            _logger.LogInformation("Actualizando día de funcionamiento {Id} para sitio {SiteId} en fecha {Date}",
                request.Id.Value, request.SiteId, request.OperatingDate.Date);

            var result = await _siteCalendarRepository.UpdateOperatingDay(request.Id.Value, request);

            if (result)
            {
                _logger.LogInformation("Día de funcionamiento {Id} actualizado exitosamente para sitio {SiteId} en fecha {Date}",
                    request.Id.Value, request.SiteId, request.OperatingDate.Date);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudo actualizar el día de funcionamiento {Id} para sitio {SiteId} en fecha {Date}",
                    request.Id.Value, request.SiteId, request.OperatingDate.Date);
                return BadRequest("No se pudo procesar la solicitud");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar día de funcionamiento {Id} para sitio {SiteId} en fecha {Date}",
                request.Id ?? 0, request.SiteId, request.OperatingDate.Date);
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

    /// <summary>
    /// Obtiene los días de la semana permitidos para un programa específico con sus nombres
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del programa</param>
    /// <returns>Lista de días permitidos con sus nombres en español e inglés</returns>
    [HttpGet("get-allowed-days-by-program-id")]
    [SwaggerOperation(Summary = "Obtiene días permitidos por programa", Description = "Devuelve los días de la semana permitidos para operar según el programa especificado con sus nombres en español e inglés.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DayOfWeekResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllowedDaysByProgramId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId.Value <= 0)
            {
                return BadRequest("El ID del programa debe ser mayor que cero");
            }

            _logger.LogInformation("Obteniendo días permitidos para el programa {ProgramId}", queryParameters.ProgramId.Value);

            var allowedDays = await _siteCalendarRepository.GetAllowedDaysByProgramId(queryParameters.ProgramId.Value);

            if (allowedDays == null || allowedDays.Count == 0)
            {
                return NotFound($"No se encontraron días permitidos para el programa {queryParameters.ProgramId.Value}");
            }

            return Ok(allowedDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días permitidos para el programa {ProgramId}: {Message}", queryParameters.ProgramId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
