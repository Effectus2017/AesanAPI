using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de comida.
/// Proporciona endpoints para la gestión completa de tipos de comida, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[ApiController]
[Route("meal-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class MealTypeController(IMealTypeRepository mealTypeRepository) : ControllerBase
{
    private readonly IMealTypeRepository _mealTypeRepository = mealTypeRepository;


    /// <summary>
    /// Obtiene un tipo de comida por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Tipo de comida</returns>
    [HttpGet("get-meal-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de comida por su ID", Description = "Devuelve un tipo de comida basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID del tipo de comida es requerido");
        }

        var result = await _mealTypeRepository.GetMealTypeById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Tipo de comida con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
    }


    /// <summary>
    /// Obtiene todos los tipos de comida
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de tipos de comida, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-meal-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de comida", Description = "Devuelve una lista de tipos de comida.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _mealTypeRepository.GetAllMealTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

        if (result == null)
        {
            return NotFound("No se encontraron tipos de comida");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo tipo de comida
    /// </summary>
    /// <param name="mealType">El tipo de comida a crear</param>
    /// <returns>El tipo de comida creado</returns>
    [HttpPost("insert-meal-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de comida", Description = "Crea un nuevo tipo de comida.")]
    public async Task<ActionResult> Insert([FromBody] DTOMealType request)
    {
        var result = await _mealTypeRepository.InsertMealType(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear el tipo de comida");
    }


    /// <summary>
    /// Actualiza un tipo de comida existente
    /// </summary>
    /// <param name="request">El tipo de comida a actualizar</param>
    /// <returns>El tipo de comida actualizado</returns>
    [HttpPut("update-meal-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de comida existente", Description = "Actualiza los datos de un tipo de comida existente.")]
    public async Task<IActionResult> Update([FromBody] DTOMealType request)
    {
        var result = await _mealTypeRepository.UpdateMealType(request);

        if (!result)
        {
            return NotFound($"Tipo de comida con ID {request.Id} no encontrado");
        }

        return NoContent();
    }

    /// <summary>
    /// Elimina un tipo de comida existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El tipo de comida eliminado</returns>
    [HttpDelete("delete-meal-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de comida existente", Description = "Elimina un tipo de comida existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _mealTypeRepository.DeleteMealType(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Tipo de comida con ID {queryParameters.Id} no encontrado");
        }

        return NoContent();
    }
}
