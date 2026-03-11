using Api.Interfaces;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las opciones de selección.
/// Proporciona endpoints para la gestión completa de opciones de selección, incluyendo creación,
/// lectura, actualización y eliminación de opciones.
/// </summary>
[ApiController]
[Route("option-selection")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class OptionSelectionController(IOptionSelectionRepository optionSelectionRepository) : ControllerBase
{
    private readonly IOptionSelectionRepository _optionSelectionRepository = optionSelectionRepository;

    /// <summary>
    /// Obtiene una opción de selección por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Opción de selección</returns>
    [HttpGet("get-option-selection-by-id")]
    [SwaggerOperation(Summary = "Obtiene una opción de selección por su ID", Description = "Devuelve una opción de selección basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
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

    /// <summary>
    /// Obtiene todas las opciones de selección
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de opciones de selección, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-option-selections")]
    [SwaggerOperation(Summary = "Obtiene todas las opciones de selección", Description = "Devuelve una lista de opciones de selección.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _optionSelectionRepository.GetAllOptionSelections(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.OptionType, queryParameters.Alls, queryParameters.ForDropdown);

        if (result == null)
        {
            return NotFound("No se encontraron opciones de selección");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene una opción de selección por su clave de opción
    /// </summary>
    /// <param name="optionKey">Clave de opción</param>
    /// <returns>Opción de selección</returns>
    [AllowAnonymous]
    [HttpGet("get-option-selection-by-option-key")]
    [SwaggerOperation(Summary = "Obtiene una opción de selección por su clave de opción", Description = "Devuelve una opción de selección basada en la clave de opción proporcionada. Accesible sin autenticación para intención de participación.")]
    public async Task<ActionResult> GetByOptionKey([FromQuery] QueryParameters queryParameters)
    {
        var result = await _optionSelectionRepository.GetOptionSelectionByOptionKey(
            queryParameters.OptionKey,
            queryParameters.Names,
            queryParameters.ForDropdown,
            queryParameters.SortByNameKeys,
            queryParameters.SortByNameENKeys,
            queryParameters.Language);

        if (result == null)
        {
            return NotFound($"Opción de selección con clave {queryParameters.OptionKey} no encontrada");
        }

        return Ok(result);
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
        if (request == null)
        {
            return BadRequest("La opción de selección es requerida");
        }

        var result = await _optionSelectionRepository.InsertOptionSelection(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear la opción de selección");
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
        var result = await _optionSelectionRepository.UpdateOptionSelection(request);

        if (!result)
        {
            return NotFound($"Opción de selección con ID {request.Id} no encontrada");
        }

        return Ok(result);
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
        var result = await _optionSelectionRepository.UpdateOptionSelectionDisplayOrder(optionSelectionId, displayOrder);

        if (!result)
        {
            return NotFound($"Opción de selección con ID {optionSelectionId} no encontrada");
        }

        return NoContent();
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
        var result = await _optionSelectionRepository.DeleteOptionSelection(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Opción de selección con ID {queryParameters.Id} no encontrada");
        }

        return NoContent();
    }
}
