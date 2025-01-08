using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("food-authority")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class FoodAuthorityController(IFoodAuthorityRepository foodAuthorityRepository, ILogger<FoodAuthorityController> logger) : ControllerBase
{
    private readonly IFoodAuthorityRepository _foodAuthorityRepository = foodAuthorityRepository;
    private readonly ILogger<FoodAuthorityController> _logger = logger;

    [HttpGet("get-all-food-authorities-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todas las autoridades alimentarias", Description = "Devuelve una lista de autoridades alimentarias.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var foodAuthorities = await _foodAuthorityRepository.GetAllFoodAuthorities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(foodAuthorities);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las autoridades alimentarias");
            return StatusCode(500, "Error interno del servidor al obtener las autoridades alimentarias");
        }
    }

    [HttpGet("get-food-authority-by-id")]
    [SwaggerOperation(Summary = "Obtiene una autoridad alimentaria por su ID", Description = "Devuelve una autoridad alimentaria basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var foodAuthority = await _foodAuthorityRepository.GetFoodAuthorityById(id);
            if (foodAuthority == null)
                return NotFound($"Autoridad alimentaria con ID {id} no encontrada");

            return Ok(foodAuthority);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la autoridad alimentaria con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener la autoridad alimentaria");
        }
    }

    [HttpPost("insert-food-authority")]
    [SwaggerOperation(Summary = "Crea una nueva autoridad alimentaria", Description = "Crea una nueva autoridad alimentaria.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOFoodAuthority foodAuthority)
    {
        try
        {
            if (ModelState.IsValid)
            {
                foodAuthority.CreatedAt = DateTime.UtcNow;
                var id = await _foodAuthorityRepository.InsertFoodAuthority(foodAuthority);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la autoridad alimentaria");
            return StatusCode(500, "Error interno del servidor al crear la autoridad alimentaria");
        }
    }

    [HttpPut("update-food-authority")]
    [SwaggerOperation(Summary = "Actualiza una autoridad alimentaria existente", Description = "Actualiza los datos de una autoridad alimentaria existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOFoodAuthority foodAuthority)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _foodAuthorityRepository.UpdateFoodAuthority(foodAuthority);
                if (!result)
                    return NotFound($"Autoridad alimentaria con ID {foodAuthority.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la autoridad alimentaria con ID {Id}", foodAuthority.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la autoridad alimentaria");
        }
    }

    [HttpDelete("delete-food-authority")]
    [SwaggerOperation(Summary = "Elimina una autoridad alimentaria existente", Description = "Elimina una autoridad alimentaria existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _foodAuthorityRepository.DeleteFoodAuthority(id);
                if (!result)
                    return NotFound($"Autoridad alimentaria con ID {id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la autoridad alimentaria con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar la autoridad alimentaria");
        }
    }
}