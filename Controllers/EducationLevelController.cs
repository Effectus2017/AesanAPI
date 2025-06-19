using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los niveles educativos.
/// Proporciona endpoints para la gestión completa de niveles educativos, incluyendo creación,
/// lectura, actualización y eliminación de niveles educativos.
/// </summary>
[Route("education-level")]
[ApiController]
public class EducationLevelController(IEducationLevelRepository educationLevelRepository, ILogger<EducationLevelController> logger) : ControllerBase
{
    private readonly IEducationLevelRepository _educationLevelRepository = educationLevelRepository;
    private readonly ILogger<EducationLevelController> _logger = logger;

    /// <summary>
    /// Obtiene un nivel educativo por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Nivel educativo</returns>
    [HttpGet("get-education-level-by-id")]
    [SwaggerOperation(Summary = "Obtiene un nivel educativo por su ID", Description = "Devuelve un nivel educativo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.GetEducationLevelById(queryParameters.Id);
                if (result == null)
                {
                    _logger.LogWarning("Nivel educativo con ID {Id} no encontrado", queryParameters.Id);
                    return NotFound($"Nivel educativo con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el nivel educativo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el nivel educativo");
        }
    }

    /// <summary>
    /// Obtiene todos los niveles educativos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Lista de niveles educativos</returns>
    [HttpGet("get-all-education-levels-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los niveles educativos", Description = "Devuelve una lista de niveles educativos.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.GetAllEducationLevels(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron niveles educativos");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los niveles educativos");
            return StatusCode(500, "Error interno del servidor al obtener los niveles educativos");
        }
    }

    /// <summary>
    /// Crea un nuevo nivel educativo
    /// </summary>
    /// <param name="request">Nivel educativo</param>
    /// <returns>Nivel educativo creado</returns>
    [HttpPost("insert-education-level")]
    [SwaggerOperation(Summary = "Crea un nuevo nivel educativo", Description = "Crea un nuevo nivel educativo.")]
    public async Task<ActionResult> Insert([FromBody] DTOEducationLevel request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.InsertEducationLevel(request);

                if (result)
                {
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear el nivel educativo");
                return BadRequest("No se pudo crear el nivel educativo");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el nivel educativo");
            return StatusCode(500, "Error interno del servidor al crear el nivel educativo");
        }
    }

    /// <summary>
    /// Actualiza un nivel educativo existente
    /// </summary>
    /// <param name="request">Nivel educativo</param>
    /// <returns>Nivel educativo actualizado</returns>
    [HttpPut("update-education-level")]
    [SwaggerOperation(Summary = "Actualiza un nivel educativo existente", Description = "Actualiza los datos de un nivel educativo existente.")]
    public async Task<IActionResult> Update([FromBody] DTOEducationLevel request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.UpdateEducationLevel(request);

                if (!result)
                {
                    _logger.LogWarning("Nivel educativo con ID {Id} no encontrado", request.Id);
                    return NotFound($"Nivel educativo con ID {request.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el nivel educativo con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el nivel educativo");
        }
    }

    /// <summary>
    /// Elimina un nivel educativo existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Nivel educativo eliminado</returns>
    [HttpDelete("delete-education-level")]
    [SwaggerOperation(Summary = "Elimina un nivel educativo existente", Description = "Elimina un nivel educativo existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.DeleteEducationLevel(queryParameters.Id);
                if (!result)
                {
                    _logger.LogWarning("Nivel educativo con ID {Id} no encontrado", queryParameters.Id);
                    return NotFound($"Nivel educativo con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el nivel educativo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el nivel educativo");
        }
    }
}