using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las instalaciones.
/// Proporciona endpoints para la gestión completa de instalaciones, incluyendo creación,
/// lectura, actualización y eliminación de registros de instalaciones.
/// </summary>
[Route("facility")]
[ApiController]
public class FacilityController(IFacilityRepository facilityRepository, ILogger<FacilityController> logger) : ControllerBase
{
    private readonly IFacilityRepository _facilityRepository = facilityRepository;
    private readonly ILogger<FacilityController> _logger = logger;

    /// <summary>
    /// Obtiene una instalación por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La instalación si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-facility-by-id")]
    [SwaggerOperation(Summary = "Obtiene una instalación por su ID", Description = "Devuelve una instalación basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo instalación por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID de la instalación es requerido");
                }

                var result = await _facilityRepository.GetFacilityById(queryParameters.Id);

                if (result == null)
                {
                    _logger.LogWarning("Instalación con ID {Id} no encontrada", queryParameters.Id);
                    return NotFound($"Instalación con ID {queryParameters.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la instalación con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener la instalación");
        }
    }

    /// <summary>
    /// Obtiene todas las instalaciones con opciones de filtrado y paginación
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de instalaciones, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-facilities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las instalaciones", Description = "Devuelve una lista de instalaciones.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.GetAllFacilities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No se encontraron instalaciones");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las instalaciones");
            return StatusCode(500, "Error interno del servidor al obtener las instalaciones");
        }
    }

    /// <summary>
    /// Crea una nueva instalación
    /// </summary>
    /// <param name="facility">La instalación a crear</param>
    /// <returns>La instalación creada si la operación es exitosa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPost("insert-facility")]
    [SwaggerOperation(Summary = "Crea una nueva instalación", Description = "Crea una nueva instalación.")]
    public async Task<ActionResult> Insert([FromBody] DTOFacility request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.InsertFacility(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo crear la instalación");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la instalación");
            return StatusCode(500, "Error interno del servidor al crear la instalación");
        }
    }

    /// <summary>
    /// Actualiza una instalación existente
    /// </summary>
    /// <param name="facility">La instalación a actualizar</param>
    /// <returns>La instalación actualizada si la operación es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-facility")]
    [SwaggerOperation(Summary = "Actualiza una instalación existente", Description = "Actualiza los datos de una instalación existente.")]
    public async Task<IActionResult> Update([FromBody] DTOFacility request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.UpdateFacility(request);

                if (!result)
                {
                    _logger.LogWarning("Instalación con ID {Id} no encontrada", request.Id);
                    return NotFound($"Instalación con ID {request.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la instalación con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la instalación");
        }
    }

    /// <summary>
    /// Elimina una instalación existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>NoContent si se elimina exitosamente, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpDelete("delete-facility")]
    [SwaggerOperation(Summary = "Elimina una instalación existente", Description = "Elimina una instalación existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.DeleteFacility(queryParameters.Id);

                if (!result)
                {
                    _logger.LogWarning("Instalación con ID {Id} no encontrada", queryParameters.Id);
                    return NotFound($"Instalación con ID {queryParameters.Id} no encontrada");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la instalación con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar la instalación");
        }
    }
}