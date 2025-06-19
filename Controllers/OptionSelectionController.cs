using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las opciones de selección.
/// Proporciona endpoints para la gestión completa de opciones de selección, incluyendo creación,
/// lectura, actualización y eliminación de opciones.
/// </summary>
[Route("option-selection")]
[ApiController]
public class OptionSelectionController(IOptionSelectionRepository optionSelectionRepository, ILogger<OptionSelectionController> logger) : ControllerBase
{
    private readonly IOptionSelectionRepository _optionSelectionRepository = optionSelectionRepository;
    private readonly ILogger<OptionSelectionController> _logger = logger;

    /// <summary>
    /// Obtiene una opción de selección por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Opción de selección</returns>
    [HttpGet("get-option-selection-by-id")]
    [SwaggerOperation(Summary = "Obtiene una opción de selección por su ID", Description = "Devuelve una opción de selección basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo opción de selección por ID: {Id}", queryParameters.OptionSelectionId);

                if (queryParameters.OptionSelectionId == 0)
                {
                    return BadRequest("El ID de la opción de selección es requerido");
                }

                var result = await _optionSelectionRepository.GetOptionSelectionById(queryParameters.OptionSelectionId);

                if (result == null)
                {
                    return NotFound($"Opción de selección con ID {queryParameters.OptionSelectionId} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la opción de selección con ID {Id}", queryParameters.OptionSelectionId);
            return StatusCode(500, "Error interno del servidor al obtener la opción de selección");
        }
    }

    /// <summary>
    /// Obtiene todas las opciones de selección
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de opciones de selección, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-option-selections")]
    [SwaggerOperation(Summary = "Obtiene todas las opciones de selección", Description = "Devuelve una lista de opciones de selección.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todas las opciones de selección");

                var result = await _optionSelectionRepository.GetAllOptionSelections(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.OptionType, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron opciones de selección");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las opciones de selección");
            return StatusCode(500, "Error interno del servidor al obtener las opciones de selección");
        }
    }

    /// <summary>
    /// Obtiene una opción de selección por su clave de opción
    /// </summary>
    /// <param name="optionKey">Clave de opción</param>
    /// <returns>Opción de selección</returns>
    [HttpGet("get-option-selection-by-option-key")]
    [SwaggerOperation(Summary = "Obtiene una opción de selección por su clave de opción", Description = "Devuelve una opción de selección basada en la clave de opción proporcionada.")]
    public async Task<ActionResult> GetByOptionKey([FromQuery] string optionKey)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _optionSelectionRepository.GetOptionSelectionByOptionKey(optionKey);

                if (result == null)
                {
                    return NotFound($"Opción de selección con clave {optionKey} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la opción de selección con clave {OptionKey}", optionKey);
            return StatusCode(500, "Error interno del servidor al obtener la opción de selección");
        }
    }

    /// <summary>
    /// Crea una nueva opción de selección
    /// </summary>
    /// <param name="request">La opción de selección a crear</param>
    /// <returns>La opción de selección creada</returns>
    [HttpPost("insert-option-selection")]
    [SwaggerOperation(Summary = "Crea una nueva opción de selección", Description = "Crea una nueva opción de selección.")]
    public async Task<ActionResult> Insert([FromBody] DTOOptionSelection request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("La opción de selección es requerida");
                }

                var result = await _optionSelectionRepository.InsertOptionSelection(request);

                if (result)
                {
                    _logger.LogInformation("Opción de selección creada con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear la opción de selección");
                return BadRequest("No se pudo crear la opción de selección");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la opción de selección");
            return StatusCode(500, "Error interno del servidor al crear la opción de selección");
        }
    }

    /// <summary>
    /// Actualiza una opción de selección existente
    /// </summary>
    /// <param name="request">La opción de selección a actualizar</param>
    /// <returns>La opción de selección actualizada</returns>
    [HttpPut("update-option-selection")]
    [SwaggerOperation(Summary = "Actualiza una opción de selección existente", Description = "Actualiza los datos de una opción de selección existente.")]
    public async Task<IActionResult> Update([FromBody] DTOOptionSelection request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _optionSelectionRepository.UpdateOptionSelection(request);

                if (!result)
                {
                    _logger.LogWarning("Opción de selección con ID {Id} no encontrada", request.Id);
                    return NotFound($"Opción de selección con ID {request.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la opción de selección con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la opción de selección");
        }
    }

    /// <summary>
    /// Actualiza el orden de visualización de una opción de selección
    /// </summary>
    /// <param name="optionSelectionId">ID de la opción de selección</param>
    /// <param name="displayOrder">Orden de visualización</param>
    /// <returns>La opción de selección actualizada</returns>
    [HttpPut("update-option-selection-display-order")]
    [SwaggerOperation(Summary = "Actualiza el orden de visualización de una opción de selección", Description = "Actualiza el orden de visualización de una opción de selección existente.")]
    public async Task<IActionResult> UpdateDisplayOrder([FromQuery] int optionSelectionId, [FromQuery] int displayOrder)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _optionSelectionRepository.UpdateOptionSelectionDisplayOrder(optionSelectionId, displayOrder);

                if (!result)
                {
                    return NotFound($"Opción de selección con ID {optionSelectionId} no encontrada");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización de la opción de selección con ID {Id}", optionSelectionId);
            return StatusCode(500, "Error interno del servidor al actualizar el orden de visualización de la opción de selección");
        }
    }

    /// <summary>
    /// Elimina una opción de selección existente
    /// </summary>
    /// <param name="id">ID de la opción de selección</param>
    /// <returns>La opción de selección eliminada</returns>
    [HttpDelete("delete-option-selection")]
    [SwaggerOperation(Summary = "Elimina una opción de selección existente", Description = "Elimina una opción de selección existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _optionSelectionRepository.DeleteOptionSelection(queryParameters.Id);

                if (!result)
                {
                    _logger.LogWarning("Opción de selección con ID {Id} no encontrada", queryParameters.Id);
                    return NotFound($"Opción de selección con ID {queryParameters.Id} no encontrada");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la opción de selección con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar la opción de selección");
        }
    }
}


