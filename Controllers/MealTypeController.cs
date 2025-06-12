using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de comida.
/// Proporciona endpoints para la gestión completa de tipos de comida, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[Route("meal-type")]
[ApiController]
public class MealTypeController(IMealTypeRepository mealTypeRepository, ILogger<MealTypeController> logger) : ControllerBase
{
    private readonly IMealTypeRepository _mealTypeRepository = mealTypeRepository;
    private readonly ILogger<MealTypeController> _logger = logger;


    /// <summary>
    /// Obtiene un tipo de comida por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Tipo de comida</returns>
    [HttpGet("get-meal-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de comida por su ID", Description = "Devuelve un tipo de comida basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo tipo de comida por ID: {Id}", queryParameters.Id);

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

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de comida con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de comida");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.GetAllMealTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No se encontraron tipos de comida");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de comida");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de comida");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.InsertMealType(request);

                if (result)
                {
                    _logger.LogInformation("Tipo de comida creado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear el tipo de comida");
                return BadRequest("No se pudo crear el tipo de comida");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de comida");
            return StatusCode(500, "Error interno del servidor al crear el tipo de comida");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.UpdateMealType(request);

                if (!result)
                {
                    _logger.LogWarning("Tipo de comida con ID {Id} no encontrado", request.Id);
                    return NotFound($"Tipo de comida con ID {request.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de comida con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de comida");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _mealTypeRepository.DeleteMealType(queryParameters.Id);

                if (!result)
                {
                    _logger.LogWarning("Tipo de comida con ID {Id} no encontrado", queryParameters.Id);
                    return NotFound($"Tipo de comida con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de comida con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de comida");
        }
    }
}