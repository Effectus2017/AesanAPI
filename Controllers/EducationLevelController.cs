using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("education-level")]
public class EducationLevelController(IEducationLevelRepository educationLevelRepository, ILogger<EducationLevelController> logger) : ControllerBase
{
    private readonly IEducationLevelRepository _educationLevelRepository = educationLevelRepository;
    private readonly ILogger<EducationLevelController> _logger = logger;

    [HttpGet("get-all-education-levels-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los niveles educativos", Description = "Devuelve una lista de niveles educativos.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var educationLevels = await _educationLevelRepository.GetAllEducationLevels(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(educationLevels);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los niveles educativos");
            return StatusCode(500, "Error interno del servidor al obtener los niveles educativos");
        }
    }

    [HttpGet("get-education-level-by-id")]
    [SwaggerOperation(Summary = "Obtiene un nivel educativo por su ID", Description = "Devuelve un nivel educativo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var educationLevel = await _educationLevelRepository.GetEducationLevelById(id);
            if (educationLevel == null)
                return NotFound($"Nivel educativo con ID {id} no encontrado");

            return Ok(educationLevel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el nivel educativo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el nivel educativo");
        }
    }

    [HttpPost("insert-education-level")]
    [SwaggerOperation(Summary = "Crea un nuevo nivel educativo", Description = "Crea un nuevo nivel educativo.")]
    public async Task<ActionResult> Insert([FromBody] DTOEducationLevel educationLevel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.InsertEducationLevel(educationLevel);
                if (result)
                    return CreatedAtAction(nameof(GetById), new { id = educationLevel.Id }, educationLevel);

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

    [HttpPut("update-education-level")]
    [SwaggerOperation(Summary = "Actualiza un nivel educativo existente", Description = "Actualiza los datos de un nivel educativo existente.")]
    public async Task<IActionResult> Update([FromBody] DTOEducationLevel educationLevel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.UpdateEducationLevel(educationLevel);
                if (!result)
                    return NotFound($"Nivel educativo con ID {educationLevel.Id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el nivel educativo con ID {Id}", educationLevel.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el nivel educativo");
        }
    }

    [HttpDelete("delete-education-level")]
    [SwaggerOperation(Summary = "Elimina un nivel educativo existente", Description = "Elimina un nivel educativo existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _educationLevelRepository.DeleteEducationLevel(id);
                if (!result)
                    return NotFound($"Nivel educativo con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el nivel educativo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el nivel educativo");
        }
    }
}