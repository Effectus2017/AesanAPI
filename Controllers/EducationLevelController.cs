using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los niveles educativos.
/// Proporciona endpoints para la gestión completa de niveles educativos, incluyendo creación,
/// lectura, actualización y eliminación de niveles educativos.
/// </summary>
[ApiController]
[Route("education-level")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class EducationLevelController(IEducationLevelRepository educationLevelRepository) : ControllerBase
{
    private readonly IEducationLevelRepository _educationLevelRepository = educationLevelRepository;

    /// <summary>
    /// Obtiene un nivel educativo por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Nivel educativo</returns>
    [HttpGet("get-education-level-by-id")]
    [SwaggerOperation(Summary = "Obtiene un nivel educativo por su ID", Description = "Devuelve un nivel educativo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        var result = await _educationLevelRepository.GetEducationLevelById(queryParameters.Id);
        if (result == null)
        {
            return NotFound($"Nivel educativo con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
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
        var result = await _educationLevelRepository.GetAllEducationLevels(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.ForDropdown);

        if (result == null)
        {
            return NotFound("No se encontraron niveles educativos");
        }

        return Ok(result);
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
        var result = await _educationLevelRepository.InsertEducationLevel(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear el nivel educativo");
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
        var result = await _educationLevelRepository.UpdateEducationLevel(request);

        if (!result)
        {
            return NotFound($"Nivel educativo con ID {request.Id} no encontrado");
        }

        return NoContent();
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
        var result = await _educationLevelRepository.DeleteEducationLevel(queryParameters.Id);
        if (!result)
        {
            return NotFound($"Nivel educativo con ID {queryParameters.Id} no encontrado");
        }

        return NoContent();
    }
}
