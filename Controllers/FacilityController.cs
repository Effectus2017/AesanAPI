using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las instalaciones.
/// Proporciona endpoints para la gestión completa de instalaciones, incluyendo creación,
/// lectura, actualización y eliminación de registros de instalaciones.
/// </summary>
[ApiController]
[Route("facility")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class FacilityController(IFacilityRepository facilityRepository) : ControllerBase
{
    private readonly IFacilityRepository _facilityRepository = facilityRepository;

    /// <summary>
    /// Obtiene una instalación por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La instalación si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-facility-by-id")]
    [SwaggerOperation(Summary = "Obtiene una instalación por su ID", Description = "Devuelve una instalación basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la instalación es requerido");
        }

        var result = await _facilityRepository.GetFacilityById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Instalación con ID {queryParameters.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todas las instalaciones con opciones de filtrado y paginación
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de instalaciones, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-facilities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las instalaciones", Description = "Devuelve una lista de instalaciones.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _facilityRepository.GetAllFacilities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

        if (result == null)
        {
            return NotFound("No se encontraron instalaciones");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva instalación
    /// </summary>
    /// <param name="facility">La instalación a crear</param>
    /// <returns>La instalación creada si la operación es exitosa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPost("insert-facility")]
    [SwaggerOperation(Summary = "Crea una nueva instalación", Description = "Crea una nueva instalación.")]
    public async Task<ActionResult> Insert([FromBody] DTOFacility request)
    {
        var result = await _facilityRepository.InsertFacility(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear la instalación");
    }

    /// <summary>
    /// Actualiza una instalación existente
    /// </summary>
    /// <param name="facility">La instalación a actualizar</param>
    /// <returns>La instalación actualizada si la operación es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-facility")]
    [SwaggerOperation(Summary = "Actualiza una instalación existente", Description = "Actualiza los datos de una instalación existente.")]
    public async Task<IActionResult> Update([FromBody] DTOFacility request)
    {
        var result = await _facilityRepository.UpdateFacility(request);

        if (!result)
        {
            return NotFound($"Instalación con ID {request.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina una instalación existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>NoContent si se elimina exitosamente, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpDelete("delete-facility")]
    [SwaggerOperation(Summary = "Elimina una instalación existente", Description = "Elimina una instalación existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _facilityRepository.DeleteFacility(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Instalación con ID {queryParameters.Id} no encontrada");
        }

        return NoContent();
    }
}
