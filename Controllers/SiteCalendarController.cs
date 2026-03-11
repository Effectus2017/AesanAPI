using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
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
[ValidateModelState]
public class SiteCalendarController(ISiteCalendarRepository siteCalendarRepository) : Controller
{
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

        var result = await _siteCalendarRepository.GetOperatingDays(
            queryParameters.SiteId.Value,
            queryParameters.Month,
            queryParameters.Year
        );
        return Ok(result);
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
        var newId = await _siteCalendarRepository.CreateOperatingDay(request);

        if (newId.HasValue)
        {
            return Ok(new { id = newId.Value, success = true });
        }
        else
        {
            return BadRequest("No se pudo procesar la solicitud");
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
        if (!request.Id.HasValue || request.Id.Value <= 0)
        {
            return BadRequest("El ID del día de funcionamiento es requerido para actualizar");
        }

        var result = await _siteCalendarRepository.UpdateOperatingDay(request.Id.Value, request);

        if (result)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudo procesar la solicitud");
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
        if (siteId <= 0)
        {
            return BadRequest("El ID del sitio debe ser mayor a 0");
        }

        if (requests == null || !requests.Any())
        {
            return BadRequest("La lista de días de funcionamiento no puede estar vacía");
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
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudieron procesar todas las solicitudes");
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
        if (id <= 0)
        {
            return BadRequest("El ID debe ser mayor a 0");
        }

        var result = await _siteCalendarRepository.DeleteOperatingDay(id);

        if (result)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudo eliminar el día de funcionamiento");
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
        if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId.Value <= 0)
        {
            return BadRequest("El ID del programa debe ser mayor que cero");
        }

        var allowedDays = await _siteCalendarRepository.GetAllowedDaysByProgramId(queryParameters.ProgramId.Value);

        if (allowedDays == null || allowedDays.Count == 0)
        {
            return NotFound($"No se encontraron días permitidos para el programa {queryParameters.ProgramId.Value}");
        }

        return Ok(allowedDays);
    }
}
