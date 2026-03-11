using Api.Interfaces;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de cocina.
/// Proporciona endpoints para la gestión completa de tipos de cocina, incluyendo creación,
/// lectura, actualización y eliminación de tipos de cocina.
/// </summary>
[ApiController]
[Route("kitchen-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class KitchenTypeController(IKitchenTypeRepository kitchenTypeRepository) : ControllerBase
{
    private readonly IKitchenTypeRepository _kitchenTypeRepository = kitchenTypeRepository;

    /// <summary>
    /// Obtiene un tipo de cocina por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El tipo de cocina si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-kitchen-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de cocina por su ID", Description = "Devuelve un tipo de cocina basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID del tipo de cocina es requerido");
        }

        var result = await _kitchenTypeRepository.GetKitchenTypeById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Tipo de cocina con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de cocina con opciones de filtrado y paginación
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de tipos de cocina, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-kitchen-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de cocina", Description = "Devuelve una lista de tipos de cocina.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _kitchenTypeRepository.GetAllKitchenTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

        if (result == null)
        {
            return NotFound("No se encontraron tipos de cocina");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo tipo de cocina
    /// </summary>
    /// <param name="type">El tipo de cocina a crear</param>
    /// <returns>El tipo de cocina creado si la operación es exitosa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPost("insert-kitchen-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de cocina", Description = "Crea un nuevo tipo de cocina.")]
    public async Task<ActionResult> Insert([FromBody] KitchenTypeRequest request)
    {
        var result = await _kitchenTypeRepository.InsertKitchenType(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear el tipo de cocina");
    }

    /// <summary>
    /// Actualiza un tipo de cocina existente
    /// </summary>
    /// <param name="type">El tipo de cocina a actualizar</param>
    /// <returns>El tipo de cocina actualizado si la operación es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-kitchen-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de cocina existente", Description = "Actualiza los datos de un tipo de cocina existente.")]
    public async Task<IActionResult> Update([FromBody] DTOKitchenType request)
    {
        var result = await _kitchenTypeRepository.UpdateKitchenType(request);

        if (!result)
        {
            return NotFound($"Tipo de cocina con ID {request.Id} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de cocina
    /// </summary>
    /// <param name="kitchenTypeId">ID del tipo de cocina a actualizar</param>
    /// <param name="displayOrder">Nuevo orden de visualización</param>
    /// <returns>NoContent si la actualización es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-kitchen-type-display-order")]
    [SwaggerOperation(Summary = "Actualiza el orden de visualización de un tipo de cocina", Description = "Actualiza el orden de visualización de un tipo de cocina existente.")]
    public async Task<IActionResult> UpdateDisplayOrder([FromQuery] int kitchenTypeId, [FromQuery] int displayOrder)
    {
        var result = await _kitchenTypeRepository.UpdateKitchenTypeDisplayOrder(kitchenTypeId, displayOrder);

        if (!result)
        {
            return NotFound($"Tipo de cocina con ID {kitchenTypeId} no encontrado");
        }

        return NoContent();
    }

    /// <summary>
    /// Elimina un tipo de cocina existente
    /// </summary>
    /// <param name="id">ID del tipo de cocina a eliminar</param>
    /// <returns>NoContent si se elimina exitosamente, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpDelete("delete-kitchen-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de cocina existente", Description = "Elimina un tipo de cocina existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _kitchenTypeRepository.DeleteKitchenType(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Tipo de cocina con ID {queryParameters.Id} no encontrado");
        }

        return NoContent();
    }

    /// <summary>
    /// Obtiene los tipos de cocina válidos para un programa
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del programa</param>
    /// <returns>Lista de tipos de cocina válidos para el programa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-kitchen-types-by-program")]
    [SwaggerOperation(Summary = "Obtiene tipos de cocina por programa", Description = "Devuelve los tipos de cocina válidos para un programa específico.")]
    public async Task<ActionResult> GetKitchenTypesByProgram([FromQuery] QueryParameters queryParameters)
    {
        if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId == 0)
        {
            return BadRequest("El ID del programa es requerido");
        }

        var result = await _kitchenTypeRepository.GetKitchenTypesByProgram(queryParameters.ProgramId.Value);

        return Ok(result);
    }

    /// <summary>
    /// Obtiene los tipos de cocina válidos para un tipo de grupo y programa específicos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del tipo de grupo y del programa</param>
    /// <returns>Lista de tipos de cocina válidos para el tipo de grupo y programa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-kitchen-types-by-group-type")]
    [SwaggerOperation(Summary = "Obtiene tipos de cocina por tipo de grupo y programa", Description = "Devuelve los tipos de cocina válidos para un tipo de grupo y programa específicos.")]
    public async Task<ActionResult> GetKitchenTypesByGroupType([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.GroupTypeId == 0)
        {
            return BadRequest("El ID del tipo de grupo es requerido");
        }

        if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId == 0)
        {
            return BadRequest("El ID del programa es requerido");
        }

        var result = await _kitchenTypeRepository.GetKitchenTypesByGroupType(queryParameters.GroupTypeId, queryParameters.ProgramId.Value);

        return Ok(result);
    }
}
