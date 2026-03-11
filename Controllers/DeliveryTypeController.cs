using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de entrega.
/// Proporciona endpoints para la gestión completa de tipos de entrega, incluyendo creación,
/// lectura, actualización y eliminación de tipos de entrega.
/// </summary>
[ApiController]
[Route("delivery-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class DeliveryTypeController(IDeliveryTypeRepository deliveryTypeRepository) : ControllerBase
{
    private readonly IDeliveryTypeRepository _deliveryTypeRepository = deliveryTypeRepository;

    /// <summary>
    /// Obtiene un tipo de entrega por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a obtener.</param>
    /// <returns>El tipo de entrega encontrado o null si no se encuentra.</returns>
    [HttpGet("get-delivery-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de entrega por su ID", Description = "Devuelve un tipo de entrega basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        var result = await _deliveryTypeRepository.GetDeliveryTypeById(queryParameters.Id);
        if (result == null)
        {
            return NotFound($"Tipo de entrega con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de entrega.
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta.</param>
    /// <returns>Una lista de tipos de entrega y el total.</returns>
    [HttpGet("get-all-delivery-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de entrega", Description = "Devuelve una lista de tipos de entrega.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _deliveryTypeRepository.GetAllDeliveryTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.ForDropdown);

        if (result == null)
        {
            return NotFound("No se encontraron tipos de entrega");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo tipo de entrega.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a crear.</param>
    /// <returns>El tipo de entrega creado.</returns>
    [HttpPost("insert-delivery-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de entrega", Description = "Crea un nuevo tipo de entrega.")]
    public async Task<ActionResult> Insert([FromBody] DeliveryTypeRequest request)
    {
        var result = await _deliveryTypeRepository.InsertDeliveryType(request);

        if (result)
        {
            return Ok(request);
        }

        return BadRequest("No se pudo crear el tipo de entrega");
    }

    /// <summary>
    /// Actualiza un tipo de entrega existente.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a actualizar.</param>
    /// <returns>El tipo de entrega actualizado.</returns>
    [HttpPut("update-delivery-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de entrega existente", Description = "Actualiza los datos de un tipo de entrega existente.")]
    public async Task<IActionResult> Update([FromBody] DeliveryTypeRequest request)
    {
        var result = await _deliveryTypeRepository.UpdateDeliveryType(request);

        if (!result)
        {
            return NotFound($"Tipo de entrega con ID {request.Id} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina un tipo de entrega existente.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    [HttpDelete("delete-delivery-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de entrega existente", Description = "Elimina un tipo de entrega existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _deliveryTypeRepository.DeleteDeliveryType(queryParameters.Id);
        if (!result)
        {
            return NotFound($"Tipo de entrega con ID {queryParameters.Id} no encontrado");
        }

        return NoContent();
    }

    /// <summary>
    /// Obtiene los tipos de entrega válidos para un programa específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del programa</param>
    /// <returns>Lista de tipos de entrega válidos para el programa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-delivery-types-by-program")]
    [SwaggerOperation(Summary = "Obtiene tipos de entrega por programa", Description = "Devuelve los tipos de entrega válidos para un programa específico.")]
    public async Task<ActionResult> GetDeliveryTypesByProgram([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.ProgramId == 0 || !queryParameters.ProgramId.HasValue)
        {
            return BadRequest("El ID del programa es requerido");
        }

        var result = await _deliveryTypeRepository.GetDeliveryTypesByProgram(queryParameters.ProgramId.Value);

        return Ok(result);
    }

    /// <summary>
    /// Obtiene los tipos de entrega válidos para un tipo de grupo específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del tipo de grupo</param>
    /// <returns>Lista de tipos de entrega válidos para el tipo de grupo con información de RequiresPermission, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-delivery-types-by-group-type")]
    [SwaggerOperation(Summary = "Obtiene tipos de entrega por tipo de grupo", Description = "Devuelve los tipos de entrega válidos para un tipo de grupo específico con información sobre si requieren permiso.")]
    public async Task<ActionResult> GetDeliveryTypesByGroupType([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.GroupTypeId == 0)
        {
            return BadRequest("El ID del tipo de grupo es requerido");
        }

        var result = await _deliveryTypeRepository.GetDeliveryTypesByGroupType(queryParameters.GroupTypeId, queryParameters.ProgramId);

        return Ok(result);
    }
}
