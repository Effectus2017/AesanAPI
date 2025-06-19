using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de organización.
/// Proporciona endpoints para la gestión completa de tipos de organización, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[Route("organization-type")]
[ApiController]

public class OrganizationTypeController(IOrganizationTypeRepository organizationTypeRepository, ILogger<OrganizationTypeController> logger) : ControllerBase
{
    private readonly IOrganizationTypeRepository _organizationTypeRepository = organizationTypeRepository;
    private readonly ILogger<OrganizationTypeController> _logger = logger;

    /// <summary>
    /// Obtiene un tipo de organización por su ID
    /// </summary>
    /// <param name="id">ID del tipo de organización</param>
    /// <returns>Tipo de organización</returns>
    [HttpGet("get-organization-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de organización por su ID", Description = "Devuelve un tipo de organización basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo tipo de organización por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del tipo de organización es requerido");
                }

                var result = await _organizationTypeRepository.GetOrganizationTypeById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Tipo de organización con ID {queryParameters.Id} no encontrado");
                }
                else
                {
                    return Ok(result);
                }
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de organización con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de organización");
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de organización
    /// </summary>
    /// <param name="take">El número de tipos de organización a obtener</param>
    /// <param name="skip">El número de tipos de organización a saltar</param>
    /// <param name="name">El nombre del tipo de organización</param>
    /// <param name="alls">Indica si se deben obtener todos los tipos de organización</param>
    [HttpGet("get-all-organization-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de organización", Description = "Devuelve una lista de tipos de organización.")]
    public async Task<ActionResult> GetAllFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.GetAllOrganizationTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron tipos de organización");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de organización");
            return StatusCode(500, "Error al obtener todos los tipos de organización");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de organización
    /// </summary>
    /// <param name="request">Tipo de organización a crear</param>
    /// <returns>Tipo de organización creado</returns>
    [HttpPost("insert-organization-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de organización", Description = "Crea un nuevo tipo de organización.")]
    public async Task<ActionResult> Insert([FromBody] OrganizationTypeRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.InsertOrganizationType(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo crear el tipo de organización");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de organización");
            return StatusCode(500, "Error al crear el tipo de organización");
        }
    }

    /// <summary>
    /// Actualiza un tipo de organización existente
    /// </summary>
    /// <param name="request">Tipo de organización a actualizar</param>
    /// <returns>Tipo de organización actualizado</returns>
    [HttpPut("update-organization-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de organización existente", Description = "Actualiza los datos de un tipo de organización existente.")]
    public async Task<IActionResult> Update([FromBody] DTOOrganizationType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.UpdateOrganizationType(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo actualizar el tipo de organización");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de organización con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de organización");
        }
    }

    /// <summary>
    /// Elimina un tipo de organización existente
    /// </summary>
    /// <param name="id">ID del tipo de organización a eliminar</param>
    /// <returns>NoContent si la eliminación es exitosa, NotFound si no existe</returns>
    [HttpDelete("delete-organization-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de organización existente", Description = "Elimina un tipo de organización existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.DeleteOrganizationType(queryParameters.Id);

                if (!result)
                {
                    return NotFound($"Tipo de organización con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de organización con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de organización");
        }
    }
}