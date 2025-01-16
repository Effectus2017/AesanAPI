using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("meal-type")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de comida.
/// Proporciona endpoints para la gestión completa de tipos de comida, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
public class MealTypeController(IMealTypeRepository mealTypeRepository, ILogger<MealTypeController> logger) : ControllerBase
{
    private readonly IMealTypeRepository _mealTypeRepository = mealTypeRepository;
    private readonly ILogger<MealTypeController> _logger = logger;

    [HttpGet("get-all-meal-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de comida", Description = "Devuelve una lista de tipos de comida.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var mealTypes = await _mealTypeRepository.GetAllMealTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(mealTypes);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de comida");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de comida");
        }
    }

    [HttpGet("get-meal-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de comida por su ID", Description = "Devuelve un tipo de comida basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var mealType = await _mealTypeRepository.GetMealTypeById(id);
            if (mealType == null)
                return NotFound($"Tipo de comida con ID {id} no encontrado");

            return Ok(mealType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de comida con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de comida");
        }
    }

    [HttpPost("insert-meal-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de comida", Description = "Crea un nuevo tipo de comida.")]
    public async Task<ActionResult> Insert([FromBody] DTOMealType mealType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var id = await _mealTypeRepository.InsertMealType(mealType);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de comida");
            return StatusCode(500, "Error interno del servidor al crear el tipo de comida");
        }
    }

    [HttpPut("update-meal-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de comida existente", Description = "Actualiza los datos de un tipo de comida existente.")]
    public async Task<IActionResult> Update([FromBody] DTOMealType mealType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.UpdateMealType(mealType);
                if (!result)
                    return NotFound($"Tipo de comida con ID {mealType.Id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de comida con ID {Id}", mealType.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de comida");
        }
    }

    [HttpDelete("delete-meal-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de comida existente", Description = "Elimina un tipo de comida existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.DeleteMealType(id);
                if (!result)
                    return NotFound($"Tipo de comida con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de comida con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de comida");
        }
    }
}