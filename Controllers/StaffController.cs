using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Dapper;

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
                    queryParameters.ExcludeRelated,
                    queryParameters.IsList,
                    queryParameters.StaffTypeId,
                    queryParameters.AgencyId
                );
                return Ok(staff);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
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
    /// Actualiza solo la imagen del staff
    /// </summary>
    /// <param name="request">Request con el ID del staff y la nueva imagen</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-image")]
    [SwaggerOperation(Summary = "Actualiza la imagen del staff", Description = "Actualiza solo la imagen del staff sin modificar otros campos.")]
    public async Task<IActionResult> UpdateStaffImage([FromBody] StaffImageRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("La información de la imagen es requerida");
                }

                var result = await _unitOfWork.StaffRepository.UpdateStaffImage(request.StaffId, request.ImageUrl);

                if (result)
                {
                    _logger.LogInformation("Imagen del staff con ID {StaffId} actualizada exitosamente", request.StaffId);

                    var response = new
                    {
                        Success = true,
                        Message = "Imagen del staff actualizada exitosamente",
                        StaffId = request.StaffId,
                        ImageUrl = request.ImageUrl,
                        UpdatedAt = DateTime.UtcNow
                    };

                    return Ok(response);
                }

                _logger.LogWarning("No se pudo actualizar la imagen del staff con ID {StaffId}", request.StaffId);
                return BadRequest("No se pudo actualizar la imagen del staff");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la imagen del staff con ID {StaffId}", request?.StaffId);
            return StatusCode(500, "Error al actualizar la imagen del staff");
        }
    }

    /// <summary>
    /// Convierte un miembro del staff en usuario del sistema
    /// </summary>
    /// <param name="queryParameters">Parámetros que incluyen staffId y userId</param>
    /// <returns>El resultado de la conversión</returns>
    [HttpPost("convert-staff-to-user")]
    [SwaggerOperation(Summary = "Convierte un miembro del staff en usuario", Description = "Convierte un miembro del staff existente en usuario del sistema.")]
    public async Task<IActionResult> ConvertStaffToUser([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Convirtiendo miembro del staff a usuario. StaffId: {StaffId}, UserId: {UserId}", queryParameters.StaffId, queryParameters.UserId);

                var result = await _unitOfWork.StaffRepository.ConvertStaffToUser(queryParameters.StaffId, queryParameters.UserId);

                if (result)
                {
                    _logger.LogInformation("Miembro del staff convertido a usuario exitosamente. StaffId: {StaffId}", queryParameters.StaffId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo convertir el miembro del staff a usuario. StaffId: {StaffId}", queryParameters.StaffId);
                return BadRequest("No se pudo convertir el miembro del staff a usuario");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir miembro del staff a usuario. StaffId: {StaffId}", queryParameters.StaffId);
            return StatusCode(500, "Error interno del servidor al convertir miembro del staff a usuario");
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un miembro del staff
    /// </summary>
    /// <param name="queryParameters">Parámetros que incluyen staffId e isActive</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-staff-active-status")]
    [SwaggerOperation(Summary = "Actualiza el estado activo de un miembro del staff", Description = "Actualiza el estado activo de un miembro del staff específico.")]
    public async Task<IActionResult> UpdateStaffActiveStatus([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Actualizando estado activo del miembro del staff: {StaffId}, isActive: {IsActive}", queryParameters.StaffId, queryParameters.IsActive);

                var result = await _unitOfWork.StaffRepository.UpdateStaffActiveStatus(queryParameters.StaffId, queryParameters.IsActive);

                if (result)
                {
                    return Ok(new { message = "Estado activo actualizado correctamente" });
                }

                return BadRequest("No se pudo actualizar el estado activo");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo del miembro del staff");
            return StatusCode(500, "Error al actualizar el estado activo");
        }
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de una agencia específica
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>La lista de miembros del staff de la agencia</returns>
    [HttpGet("get-staff-by-agency")]
    [SwaggerOperation(Summary = "Obtiene todos los miembros del staff de una agencia específica", Description = "Devuelve una lista de todos los miembros del staff de una agencia específica con paginación y filtros.")]
    public async Task<IActionResult> GetStaffByAgency([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo miembros del staff de la agencia: {AgencyId}", queryParameters.AgencyId);

                var staff = await _unitOfWork.StaffRepository.GetStaffByAgency(
                    queryParameters.AgencyId,
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Names,
                    queryParameters.StaffTypeId
                );

                if (staff == null)
                {
                    return NotFound("No se encontraron miembros del staff para la agencia especificada");
                }

                return Ok(staff);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff de la agencia {AgencyId}", queryParameters.AgencyId);
            return StatusCode(500, "Error interno del servidor al obtener los miembros del staff de la agencia");
        }
    }
}