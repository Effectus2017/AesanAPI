using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Models.Request;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar miembros del staff
/// Proporciona endpoints para la gestión completa de miembros del staff, incluyendo creación,
/// lectura, actualización y eliminación de miembros del staff.
/// </summary>
[Route("staff")]
[ApiController]
// [Authorize]
public class StaffController(ILogger<StaffController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<StaffController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un miembro del staff por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El miembro del staff si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-staff-by-id")]
    [SwaggerOperation(Summary = "Obtiene un miembro del staff por su ID", Description = "Devuelve un miembro del staff basado en el ID proporcionado.")]
    public async Task<IActionResult> GetStaffById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo miembro del staff por ID: {Id}", queryParameters.Id);

                var staff = await _unitOfWork.StaffRepository.GetStaffById(queryParameters.Id);

                if (staff == null)
                {
                    return NotFound($"Miembro del staff con ID {queryParameters.Id} no encontrado");
                }

                return Ok(staff);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el miembro del staff con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el miembro del staff");
        }
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>La lista de miembros del staff si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-staff-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los miembros del staff de la base de datos", Description = "Devuelve una lista de todos los miembros del staff. Se pueden filtrar por múltiples nombres separados por coma.")]
    public async Task<IActionResult> GetAllStaff([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todos los miembros del staff");

                var staff = await _unitOfWork.StaffRepository.GetAllStaffFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Names,
                    queryParameters.Alls,
                    queryParameters.IsList
                );
                return Ok(staff);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff");
            return StatusCode(500, "Error al obtener los miembros del staff");
        }
    }

    /// <summary>
    /// Inserta un nuevo miembro del staff
    /// </summary>
    /// <param name="request">El miembro del staff a insertar</param>
    /// <returns>El resultado de la inserción</returns>
    [HttpPost("insert-staff")]
    [SwaggerOperation(Summary = "Inserta un nuevo miembro del staff", Description = "Crea un nuevo miembro del staff en la base de datos.")]
    public async Task<IActionResult> InsertStaff([FromBody] StaffRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El miembro del staff es requerido");
                }

                var result = await _unitOfWork.StaffRepository.InsertStaff(request);

                if (result)
                {
                    _logger.LogInformation("Miembro del staff insertado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo insertar el miembro del staff");
                return BadRequest("No se pudo insertar el miembro del staff");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el miembro del staff");
            return StatusCode(500, "Error al insertar el miembro del staff");
        }
    }

    /// <summary>
    /// Actualiza un miembro del staff existente
    /// </summary>
    /// <param name="request">El miembro del staff a actualizar</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-staff")]
    [SwaggerOperation(Summary = "Actualiza un miembro del staff existente", Description = "Actualiza un miembro del staff existente en la base de datos.")]
    public async Task<IActionResult> UpdateStaff([FromBody] StaffRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El miembro del staff es requerido");
                }

                var result = await _unitOfWork.StaffRepository.UpdateStaff(request);

                if (result)
                {
                    _logger.LogInformation("Miembro del staff actualizado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo actualizar el miembro del staff");
                return BadRequest("No se pudo actualizar el miembro del staff");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el miembro del staff");
            return StatusCode(500, "Error al actualizar el miembro del staff");
        }
    }

    /// <summary>
    /// Elimina un miembro del staff (baja lógica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la eliminación</returns>
    [HttpDelete("delete-staff")]
    [SwaggerOperation(Summary = "Elimina un miembro del staff", Description = "Realiza una baja lógica del miembro del staff en la base de datos.")]
    public async Task<IActionResult> DeleteStaff([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Eliminando miembro del staff con ID: {Id}", queryParameters.Id);

                var result = await _unitOfWork.StaffRepository.DeleteStaff(queryParameters.Id);

                if (result)
                {
                    _logger.LogInformation("Miembro del staff eliminado con ID: {Id}", queryParameters.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo eliminar el miembro del staff con ID: {Id}", queryParameters.Id);
                return BadRequest("No se pudo eliminar el miembro del staff");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el miembro del staff con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error al eliminar el miembro del staff");
        }
    }

    /// <summary>
    /// Convierte un miembro del staff en usuario del sistema
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>El resultado de la conversión</returns>
    [HttpPost("convert-staff-to-user")]
    [SwaggerOperation(Summary = "Convierte un miembro del staff en usuario", Description = "Convierte un miembro del staff existente en usuario del sistema.")]
    public async Task<IActionResult> ConvertStaffToUser([FromQuery] int staffId, [FromQuery] string userId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Convirtiendo miembro del staff a usuario. StaffId: {StaffId}, UserId: {UserId}", staffId, userId);

                var result = await _unitOfWork.StaffRepository.ConvertStaffToUser(staffId, userId);

                if (result)
                {
                    _logger.LogInformation("Miembro del staff convertido a usuario exitosamente. StaffId: {StaffId}", staffId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo convertir el miembro del staff a usuario. StaffId: {StaffId}", staffId);
                return BadRequest("No se pudo convertir el miembro del staff a usuario");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir miembro del staff a usuario. StaffId: {StaffId}", staffId);
            return StatusCode(500, "Error interno del servidor al convertir miembro del staff a usuario");
        }
    }

    /// <summary>
    /// Verifica si existe un miembro del staff principal
    /// </summary>
    /// <returns>True si existe un miembro del staff principal</returns>
    [HttpGet("has-main-staff")]
    [SwaggerOperation(Summary = "Verifica si existe un miembro del staff principal", Description = "Verifica si existe al menos un miembro del staff activo en el sistema.")]
    public async Task<IActionResult> HasMainStaff()
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Verificando si existe miembro del staff principal");

                var result = await _unitOfWork.StaffRepository.HasMainStaff();

                _logger.LogInformation("Resultado de verificación de miembro del staff principal: {Result}", result);
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe miembro del staff principal");
            return StatusCode(500, "Error interno del servidor al verificar miembro del staff principal");
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-staff-active-status")]
    [SwaggerOperation(Summary = "Actualiza el estado activo de un miembro del staff", Description = "Actualiza el estado activo de un miembro del staff específico.")]
    public async Task<IActionResult> UpdateStaffActiveStatus([FromQuery] int staffId, [FromQuery] bool isActive)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Actualizando estado activo del miembro del staff. StaffId: {StaffId}, IsActive: {IsActive}", staffId, isActive);

                var result = await _unitOfWork.StaffRepository.UpdateStaffActiveStatus(staffId, isActive);

                if (result)
                {
                    _logger.LogInformation("Estado activo del miembro del staff actualizado exitosamente. StaffId: {StaffId}", staffId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo actualizar el estado activo del miembro del staff. StaffId: {StaffId}", staffId);
                return BadRequest("No se pudo actualizar el estado activo del miembro del staff");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado activo del miembro del staff. StaffId: {StaffId}", staffId);
            return StatusCode(500, "Error interno del servidor al actualizar estado activo del miembro del staff");
        }
    }
}