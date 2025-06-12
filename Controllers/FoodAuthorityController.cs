using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las autoridades alimentarias.
/// Proporciona endpoints para la gestión completa de autoridades alimentarias, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[Route("food-authority")]
[ApiController]
public class FoodAuthorityController(IFoodAuthorityRepository foodAuthorityRepository, ILogger<FoodAuthorityController> logger) : ControllerBase
{
    private readonly IFoodAuthorityRepository _foodAuthorityRepository = foodAuthorityRepository;
    private readonly ILogger<FoodAuthorityController> _logger = logger;

    /// <summary>
    /// Obtiene una autoridad alimentaria por su ID
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria</param>
    /// <returns>La autoridad alimentaria</returns>
    [HttpGet("get-food-authority-by-id")]
    [SwaggerOperation(Summary = "Obtiene una autoridad alimentaria por su ID", Description = "Devuelve una autoridad alimentaria basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo autoridad alimentaria por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID de la autoridad alimentaria es requerido");
                }

                var result = await _foodAuthorityRepository.GetFoodAuthorityById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Autoridad alimentaria con ID {queryParameters.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la autoridad alimentaria con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener la autoridad alimentaria");
        }
    }


    /// <summary>
    /// Obtiene todas las autoridades alimentarias
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>Las autoridades alimentarias</returns>
    [HttpGet("get-all-food-authorities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las autoridades alimentarias", Description = "Devuelve una lista de autoridades alimentarias.")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _foodAuthorityRepository.GetAllFoodAuthorities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las autoridades alimentarias");
            return StatusCode(500, "Error interno del servidor al obtener las autoridades alimentarias");
        }
    }


    /// <summary>
    /// Crea una nueva autoridad alimentaria
    /// </summary>
    /// <param name="foodAuthority">La autoridad alimentaria a crear</param>
    /// <returns>La autoridad alimentaria creada</returns>
    [HttpPost("insert-food-authority")]
    [SwaggerOperation(Summary = "Crea una nueva autoridad alimentaria", Description = "Crea una nueva autoridad alimentaria.")]
    public async Task<IActionResult> Insert([FromBody] FoodAuthorityRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("La autoridad alimentaria es requerida");
                }

                var result = await _foodAuthorityRepository.InsertFoodAuthority(request);

                if (result)
                {
                    _logger.LogInformation("Autoridad alimentaria creada");
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear la autoridad alimentaria");
                return BadRequest("No se pudo crear la autoridad alimentaria");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la autoridad alimentaria");
            return StatusCode(500, "Error interno del servidor al crear la autoridad alimentaria");
        }
    }

    /// <summary>
    /// Actualiza una autoridad alimentaria existente
    /// </summary>
    /// <param name="request">La autoridad alimentaria a actualizar</param>
    /// <returns>La autoridad alimentaria actualizada</returns>
    [HttpPut("update-food-authority")]
    [SwaggerOperation(Summary = "Actualiza una autoridad alimentaria existente", Description = "Actualiza los datos de una autoridad alimentaria existente.")]
    public async Task<IActionResult> Update([FromBody] DTOFoodAuthority request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("La autoridad alimentaria es requerida");
                }

                var result = await _foodAuthorityRepository.UpdateFoodAuthority(request);
                if (!result)
                    return NotFound($"Autoridad alimentaria con ID {request.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la autoridad alimentaria con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la autoridad alimentaria");
        }
    }

    /// <summary>
    /// Elimina una autoridad alimentaria existente
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria a eliminar</param>
    /// <returns>NoContent si la eliminación fue exitosa, NotFound si la autoridad alimentaria no existe</returns>
    [HttpDelete("delete-food-authority")]
    [SwaggerOperation(Summary = "Elimina una autoridad alimentaria existente", Description = "Elimina una autoridad alimentaria existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _foodAuthorityRepository.DeleteFoodAuthority(queryParameters.Id);

                if (!result)
                {
                    return NotFound($"Autoridad alimentaria con ID {queryParameters.Id} no encontrada");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la autoridad alimentaria con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar la autoridad alimentaria");
        }
    }
}