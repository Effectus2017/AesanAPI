using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las autoridades alimentarias.
/// Proporciona endpoints para la gestión completa de autoridades alimentarias, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[ApiController]
[Route("food-authority")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class FoodAuthorityController(IFoodAuthorityRepository foodAuthorityRepository) : ControllerBase
{
    private readonly IFoodAuthorityRepository _foodAuthorityRepository = foodAuthorityRepository;

    /// <summary>
    /// Obtiene una autoridad alimentaria por su ID
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria</param>
    /// <returns>La autoridad alimentaria</returns>
    [HttpGet("get-food-authority-by-id")]
    [SwaggerOperation(Summary = "Obtiene una autoridad alimentaria por su ID", Description = "Devuelve una autoridad alimentaria basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
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


    /// <summary>
    /// Obtiene todas las autoridades alimentarias
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>Las autoridades alimentarias</returns>
    [HttpGet("get-all-food-authorities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las autoridades alimentarias", Description = "Devuelve una lista de autoridades alimentarias.")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _foodAuthorityRepository.GetAllFoodAuthorities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
        return Ok(result);
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
        if (request == null)
        {
            return BadRequest("La autoridad alimentaria es requerida");
        }

        var result = await _foodAuthorityRepository.InsertFoodAuthority(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear la autoridad alimentaria");
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
        if (request == null)
        {
            return BadRequest("La autoridad alimentaria es requerida");
        }

        var result = await _foodAuthorityRepository.UpdateFoodAuthority(request);
        if (!result)
        {
            return NotFound($"Autoridad alimentaria con ID {request.Id} no encontrada");
        }

        return NoContent();
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
        var result = await _foodAuthorityRepository.DeleteFoodAuthority(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Autoridad alimentaria con ID {queryParameters.Id} no encontrada");
        }

        return NoContent();
    }
}
