using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Errors;

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
public class StaffTypeController(ILogger<StaffTypeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<StaffTypeController> _logger = logger;
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
        try
        {
            _logger.LogInformation("Obteniendo tipo de staff por ID: {Id}", queryParameters.Id);

            var staffType = await _unitOfWork.StaffTypeRepository.GetStaffTypeById(queryParameters.Id);

            if (staffType == null)
            {
                return NotFound($"Tipo de staff con ID {queryParameters.Id} no encontrado");
            }

            return Ok(staffType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de staff con ID {Id}. Detalles: {Message}", queryParameters.Id, ex.Message);

            return StatusCode(500, new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener el tipo de staff", 500));
        }
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
        try
        {
            _logger.LogInformation("Obteniendo todos los tipos de staff. Parámetros: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}",
                queryParameters.Take, queryParameters.Skip, queryParameters.Names, queryParameters.Alls, queryParameters.IsList);

            var staffTypes = await _unitOfWork.StaffTypeRepository.GetAllStaffTypesFromDb(
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.Names,
                queryParameters.Alls,
                queryParameters.IsList
            );
            return Ok(staffTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de staff. Parámetros: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}. Detalles: {Message}",
                queryParameters.Take, queryParameters.Skip, queryParameters.Names, queryParameters.Alls, queryParameters.IsList, ex.Message);

            return StatusCode(500, new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener los tipos de staff", 500));
        }
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
        try
        {
            if (request == null)
            {
                return BadRequest("El tipo de staff es requerido");
            }

            _logger.LogInformation("Insertando nuevo tipo de staff: {Name}", request.Name);

            var result = await _unitOfWork.StaffTypeRepository.InsertStaffType(request);

            if (result)
            {
                _logger.LogInformation("Tipo de staff insertado exitosamente");
                return Ok(result);
            }

            _logger.LogWarning("No se pudo insertar el tipo de staff");
            return BadRequest("No se pudo insertar el tipo de staff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de staff. Detalles: {Message}", ex.Message);
            return StatusCode(500, new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar el tipo de staff", 500));
        }
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
        try
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
                _logger.LogInformation("Tipo de staff actualizado exitosamente con ID: {Id}", request.Id);
                return Ok(result);
            }

            _logger.LogWarning("No se pudo actualizar el tipo de staff con ID: {Id}", request.Id);
            return BadRequest("No se pudo actualizar el tipo de staff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de staff con ID {Id}. Detalles: {Message}", request.Id, ex.Message);
            return StatusCode(500, new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al actualizar el tipo de staff", 500));
        }
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
        try
        {
            _logger.LogInformation("Eliminando tipo de staff con ID: {Id}", queryParameters.Id);

            var result = await _unitOfWork.StaffTypeRepository.DeleteStaffType(queryParameters.Id);

            if (result)
            {
                _logger.LogInformation("Tipo de staff eliminado exitosamente con ID: {Id}", queryParameters.Id);
                return Ok(result);
            }

            _logger.LogWarning("No se pudo eliminar el tipo de staff con ID: {Id}", queryParameters.Id);
            return BadRequest("No se pudo eliminar el tipo de staff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de staff con ID {Id}. Detalles: {Message}", queryParameters.Id, ex.Message);

            return StatusCode(500, new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al eliminar el tipo de staff", 500));
        }
    }
}