using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar miembros del staff
/// Proporciona endpoints para la gestión completa de miembros del staff, incluyendo creación,
/// lectura, actualización y eliminación de miembros del staff.
/// </summary>
[ApiController]
[Route("staff")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class StaffController(IUnitOfWork unitOfWork) : Controller
{
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
        var staff = await _unitOfWork.StaffRepository.GetStaffById(queryParameters.Id);

        if (staff == null)
        {
            return NotFound($"Miembro del staff con ID {queryParameters.Id} no encontrado");
        }

        return Ok(staff);
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
        var result = await _unitOfWork.StaffRepository.GetAllStaffFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Name,
            queryParameters.Alls,
            queryParameters.ExcludeRelated,
            queryParameters.IsList,
            queryParameters.StaffTypeId,
            queryParameters.AgencyId
        );
        return Ok(result);
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
        if (request == null)
        {
            return BadRequest("El miembro del staff es requerido");
        }

        var result = await _unitOfWork.StaffRepository.InsertStaff(request);

        if (result > 0)
        {
            return Ok(true);
        }

        return BadRequest("No se pudo insertar el miembro del staff");
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
        if (request == null)
        {
            return BadRequest("El miembro del staff es requerido");
        }

        var result = await _unitOfWork.StaffRepository.UpdateStaff(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo actualizar el miembro del staff");
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
        var result = await _unitOfWork.StaffRepository.DeleteStaff(queryParameters.Id);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo eliminar el miembro del staff");
    }

    /// <summary>
    /// Elimina completamente un miembro del staff y todas sus relaciones, incluyendo la agencia si es propietario
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta que incluyen el ID del staff</param>
    /// <returns>True si se eliminó correctamente</returns>
    [HttpDelete("bulk-delete-staff")]
    [SwaggerOperation(Summary = "Elimina completamente un miembro del staff y todas sus relaciones", Description = "Elimina permanentemente un miembro del staff, todas sus relaciones (StaffRelationship, SiteStaff), y si es propietario de una agencia, también elimina la agencia completa.")]
    public async Task<IActionResult> BulkDeleteStaff([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID del staff es requerido");
        }

        var result = await _unitOfWork.StaffRepository.BulkDeleteStaff(queryParameters.Id);

        if (result)
        {
            return Ok(new { success = true, message = "Staff y todas sus relaciones eliminadas exitosamente" });
        }

        return BadRequest("No se pudo eliminar el miembro del staff");
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
        if (request == null)
        {
            return BadRequest("La información de la imagen es requerida");
        }

        var result = await _unitOfWork.StaffRepository.UpdateStaffImage(request.StaffId, request.ImageUrl);

        if (result)
        {
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

        return BadRequest("No se pudo actualizar la imagen del staff");
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
        var result = await _unitOfWork.StaffRepository.ConvertStaffToUser(queryParameters.StaffId, queryParameters.UserId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo convertir el miembro del staff a usuario");
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
        var result = await _unitOfWork.StaffRepository.UpdateStaffActiveStatus(
            queryParameters.StaffId,
            queryParameters.IsActive);

        if (result)
        {
            return Ok(new { message = "Estado activo actualizado correctamente" });
        }

        return BadRequest("No se pudo actualizar el estado activo");
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
        var result = await _unitOfWork.StaffRepository.GetStaffByAgency(
            queryParameters.AgencyId,
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Name,
            queryParameters.StaffTypeId
        );

        if (result == null)
        {
            return NotFound("No se encontraron miembros del staff para la agencia especificada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene el historial de auditoría de un miembro del staff
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el StaffId</param>
    /// <returns>Lista de registros de auditoría del staff</returns>
    [HttpGet("get-staff-audit-history")]
    [SwaggerOperation(Summary = "Obtiene el historial de auditoría de un miembro del staff", Description = "Devuelve el historial completo de cambios realizados en un miembro del staff.")]
    public async Task<IActionResult> GetStaffAuditHistory([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.StaffId <= 0)
        {
            return BadRequest("El ID del staff es requerido y debe ser mayor a 0.");
        }

        var result = await _unitOfWork.StaffRepository.GetStaffAuditHistory(queryParameters.StaffId, queryParameters.Take);

        if (result == null || result.Count == 0)
        {
            return NotFound("No se encontró historial de auditoría para el staff especificado.");
        }

        return Ok(result);
    }
}
