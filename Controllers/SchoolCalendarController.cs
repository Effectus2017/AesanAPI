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
/// Controlador para gestionar el calendario de funcionamiento de escuelas
/// Proporciona endpoints para la gestión completa de días de funcionamiento,
/// incluyendo consulta, creación, actualización y eliminación de días específicos.
/// </summary>
[Route("school-calendar")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SchoolCalendarController(ILogger<SchoolCalendarController> logger, ISchoolCalendarRepository schoolCalendarRepository, DapperContext context) : Controller
{
    private readonly ILogger<SchoolCalendarController> _logger = logger;
    private readonly ISchoolCalendarRepository _schoolCalendarRepository = schoolCalendarRepository ?? throw new ArgumentNullException(nameof(schoolCalendarRepository));
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Obtiene todos los días de funcionamiento de una escuela específica
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID de la escuela</param>
    /// <returns>Información de la escuela y sus días de funcionamiento</returns>
    [HttpGet("get-operating-days")]
    [SwaggerOperation(Summary = "Obtiene días de funcionamiento de una escuela", Description = "Devuelve todos los días de funcionamiento para una escuela específica.")]
    public async Task<IActionResult> GetOperatingDays([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.SchoolId <= 0)
            {
                return BadRequest("El ID de la escuela debe ser mayor a 0");
            }

            _logger.LogInformation("Obteniendo días de funcionamiento para la escuela {SchoolId}", queryParameters.SchoolId);

            // Verificar que la escuela existe
            var schoolExists = await _schoolCalendarRepository.SchoolExists(queryParameters.SchoolId.Value);
            if (!schoolExists)
            {
                _logger.LogWarning("Escuela {SchoolId} no encontrada", queryParameters.SchoolId);
                return NotFound($"Escuela con ID {queryParameters.SchoolId} no encontrada");
            }

            var result = await _schoolCalendarRepository.GetOperatingDays(queryParameters.SchoolId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días de funcionamiento para la escuela {SchoolId}", queryParameters.SchoolId);
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
    public async Task<IActionResult> ToggleOperatingDay([FromBody] SchoolOperatingDayRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Alternando día de funcionamiento para escuela {SchoolId} en fecha {Date}",
                request.schoolId, request.operatingDate.Date);

            // Verificar que la escuela existe
            var schoolExists = await _schoolCalendarRepository.SchoolExists(request.schoolId);
            if (!schoolExists)
            {
                _logger.LogWarning("Escuela {SchoolId} no encontrada", request.schoolId);
                return NotFound($"Escuela con ID {request.schoolId} no encontrada");
            }

            var result = await _schoolCalendarRepository.ToggleOperatingDay(request);

            if (result)
            {
                _logger.LogInformation("Día de funcionamiento alternado exitosamente para escuela {SchoolId} en fecha {Date}",
                    request.schoolId, request.operatingDate.Date);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("No se pudo alternar el día de funcionamiento para escuela {SchoolId} en fecha {Date}",
                    request.schoolId, request.operatingDate.Date);
                return BadRequest("No se pudo procesar la solicitud");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al alternar día de funcionamiento para escuela {SchoolId} en fecha {Date}",
                request.schoolId, request.operatingDate.Date);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza múltiples días de funcionamiento en lote
    /// </summary>
    /// <param name="schoolId">ID del sitio</param>
    /// <param name="requests">Lista de días de funcionamiento a actualizar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("bulk-update-operating-days")]
    [SwaggerOperation(Summary = "Actualiza múltiples días de funcionamiento", Description = "Actualiza múltiples días de funcionamiento en una sola operación.")]
    public async Task<IActionResult> BulkUpdateOperatingDays([FromQuery] int schoolId, [FromBody] List<SchoolOperatingDayRequest> requests)
    {
        try
        {
            if (schoolId <= 0)
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

            _logger.LogInformation("Actualizando {Count} días de funcionamiento para escuela {SchoolId}",
                requests.Count, schoolId);

            // Verificar que la escuela existe
            var schoolExists = await _schoolCalendarRepository.SchoolExists(schoolId);
            if (!schoolExists)
            {
                _logger.LogWarning("Escuela {SchoolId} no encontrada", schoolId);
                return NotFound($"Escuela con ID {schoolId} no encontrada");
            }

            // Validar que todos los requests pertenecen a la misma escuela
            var invalidRequests = requests.Where(r => r.schoolId != schoolId).ToList();
            if (invalidRequests.Any())
            {
                return BadRequest("Todos los días de funcionamiento deben pertenecer a la misma escuela");
            }

            var result = await _schoolCalendarRepository.BulkUpdateOperatingDays(schoolId, requests);

            if (result)
            {
                _logger.LogInformation("Actualización en lote exitosa para escuela {SchoolId}", schoolId);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning("Actualización en lote fallida para escuela {SchoolId}", schoolId);
                return BadRequest("No se pudieron procesar todas las solicitudes");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización en lote para sitio {SchoolId}", schoolId);
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
            var existsQuery = "SELECT COUNT(1) FROM SchoolOperatingDays WHERE Id = @Id";
            var existsParameters = new DynamicParameters();
            existsParameters.Add("@Id", id, DbType.Int32);

            var exists = await dbConnection.QuerySingleAsync<int>(existsQuery, existsParameters);
            if (exists == 0)
            {
                _logger.LogWarning("Día de funcionamiento {Id} no encontrado", id);
                return NotFound($"Día de funcionamiento con ID {id} no encontrado");
            }

            var result = await _schoolCalendarRepository.DeleteOperatingDay(id);

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
