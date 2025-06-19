using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los períodos operativos.
/// Proporciona endpoints para la gestión completa de períodos operativos, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[Route("operating-period")]
[ApiController]
public class OperatingPeriodController(IOperatingPeriodRepository operatingPeriodRepository, ILogger<OperatingPeriodController> logger) : ControllerBase
{
    private readonly IOperatingPeriodRepository _operatingPeriodRepository = operatingPeriodRepository;
    private readonly ILogger<OperatingPeriodController> _logger = logger;

    /// <summary>
    /// Obtiene un período operativo por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Período operativo</returns>
    [HttpGet("get-operating-period-by-id")]
    [SwaggerOperation(Summary = "Obtiene un período operativo por su ID", Description = "Devuelve un período operativo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo período operativo por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del período operativo es requerido");
                }

                var result = await _operatingPeriodRepository.GetOperatingPeriodById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Período operativo con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el período operativo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el período operativo");
        }
    }

    /// <summary>
    /// Obtiene todos los períodos operativos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de períodos operativos, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-operating-periods-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los períodos operativos", Description = "Devuelve una lista de períodos operativos.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.GetAllOperatingPeriods(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron períodos operativos");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los períodos operativos");
            return StatusCode(500, "Error interno del servidor al obtener los períodos operativos");
        }
    }

    /// <summary>
    /// Crea un nuevo período operativo
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a crear</param>
    /// <returns>El período operativo creado</returns>
    [HttpPost("insert-operating-period")]
    [SwaggerOperation(Summary = "Crea un nuevo período operativo", Description = "Crea un nuevo período operativo.")]
    public async Task<ActionResult> Insert([FromBody] DTOOperatingPeriod request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.InsertOperatingPeriod(request);

                if (result)
                {
                    _logger.LogInformation("Período operativo creado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear el período operativo");
                return BadRequest("No se pudo crear el período operativo");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el período operativo");
            return StatusCode(500, "Error interno del servidor al crear el período operativo");
        }
    }

    /// <summary>
    /// Actualiza un período operativo existente
    /// </summary>
    /// <param name="request">El período operativo a actualizar</param>
    /// <returns>El período operativo actualizado</returns>
    [HttpPut("update-operating-period")]
    [SwaggerOperation(Summary = "Actualiza un período operativo existente", Description = "Actualiza los datos de un período operativo existente.")]
    public async Task<IActionResult> Update([FromBody] DTOOperatingPeriod request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.UpdateOperatingPeriod(request);

                if (!result)
                {
                    _logger.LogWarning("Período operativo con ID {Id} no encontrado", request.Id);
                    return NotFound($"Período operativo con ID {request.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el período operativo con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el período operativo");
        }
    }

    /// <summary>
    /// Elimina un período operativo existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El período operativo eliminado</returns>
    [HttpDelete("delete-operating-period")]
    [SwaggerOperation(Summary = "Elimina un período operativo existente", Description = "Elimina un período operativo existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.DeleteOperatingPeriod(queryParameters.Id);

                if (!result)
                {
                    _logger.LogWarning("Período operativo con ID {Id} no encontrado", queryParameters.Id);
                    return NotFound($"Período operativo con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el período operativo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el período operativo");
        }
    }
}