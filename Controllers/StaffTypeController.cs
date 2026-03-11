using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Models.Request;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar tipos de staff
/// Proporciona endpoints para la gestión completa de tipos de staff, incluyendo creación,
/// lectura, actualización y eliminación de tipos de staff.
/// </summary>
[ApiController]
[Route("staff-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class StaffTypeController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un tipo de staff por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El tipo de staff si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-staff-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de staff por su ID", Description = "Devuelve un tipo de staff basado en el ID proporcionado.")]
    public async Task<IActionResult> GetStaffTypeById([FromQuery] QueryParameters queryParameters)
    {
        var staffType = await _unitOfWork.StaffTypeRepository.GetStaffTypeById(queryParameters.Id);

        if (staffType == null)
        {
            return NotFound($"Tipo de staff con ID {queryParameters.Id} no encontrado");
        }

        return Ok(staffType);
    }

    /// <summary>
    /// Obtiene todos los tipos de staff de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>La lista de tipos de staff si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-staff-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de staff de la base de datos", Description = "Devuelve una lista de todos los tipos de staff. Se pueden filtrar por múltiples nombres separados por coma.")]
    public async Task<IActionResult> GetAllStaffTypes([FromQuery] QueryParameters queryParameters)
    {
        var staffTypes = await _unitOfWork.StaffTypeRepository.GetAllStaffTypesFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Names,
            queryParameters.Alls,
            queryParameters.ForDropdown
        );
        return Ok(staffTypes);
    }

    /// <summary>
    /// Inserta un nuevo tipo de staff
    /// </summary>
    /// <param name="request">El tipo de staff a insertar</param>
    /// <returns>El resultado de la inserción</returns>
    [HttpPost("insert-staff-type")]
    [SwaggerOperation(Summary = "Inserta un nuevo tipo de staff", Description = "Crea un nuevo tipo de staff en la base de datos.")]
    public async Task<IActionResult> InsertStaffType([FromBody] StaffTypeRequest request)
    {
        if (request == null)
        {
            return BadRequest("El tipo de staff es requerido");
        }

        var result = await _unitOfWork.StaffTypeRepository.InsertStaffType(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo insertar el tipo de staff");
    }

    /// <summary>
    /// Actualiza un tipo de staff existente
    /// </summary>
    /// <param name="request">El tipo de staff a actualizar</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-staff-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de staff existente", Description = "Actualiza un tipo de staff existente en la base de datos.")]
    public async Task<IActionResult> UpdateStaffType([FromBody] StaffTypeRequest request)
    {
        if (request == null)
        {
            return BadRequest("El tipo de staff es requerido");
        }

        if (request.Id == null || request.Id <= 0)
        {
            return BadRequest("El ID del tipo de staff es requerido y debe ser mayor que cero");
        }

        var result = await _unitOfWork.StaffTypeRepository.UpdateStaffType(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo actualizar el tipo de staff");
    }

    /// <summary>
    /// Elimina un tipo de staff (baja lógica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la eliminación</returns>
    [HttpDelete("delete-staff-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de staff", Description = "Realiza una baja lógica del tipo de staff en la base de datos.")]
    public async Task<IActionResult> DeleteStaffType([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.StaffTypeRepository.DeleteStaffType(queryParameters.Id);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo eliminar el tipo de staff");
    }
}
