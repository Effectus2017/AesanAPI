using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar las asignaciones de empleados a sitios
/// Proporciona endpoints para asignar, desasignar y consultar empleados en sitios
/// </summary>
[ApiController]
[Route("site-staff")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class SiteStaffController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene todos los empleados asignados a un sitio específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del sitio</param>
    /// <returns>Lista de empleados asignados al sitio</returns>
    [HttpGet("get-staff-by-site")]
    [SwaggerOperation(Summary = "Obtiene empleados asignados a un sitio", Description = "Devuelve todos los empleados asignados a un sitio específico.")]
    public async Task<IActionResult> GetStaffBySite([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.SiteId == null || queryParameters.SiteId == 0)
        {
            return BadRequest("El ID del sitio es requerido");
        }

        var result = await _unitOfWork.SiteStaffRepository.GetStaffBySite(queryParameters.SiteId ?? 0);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del empleado</param>
    /// <returns>Lista de sitios asignados al empleado</returns>
    [HttpGet("get-sites-by-staff")]
    [SwaggerOperation(Summary = "Obtiene sitios asignados a un empleado", Description = "Devuelve todos los sitios asignados a un empleado específico.")]
    public async Task<IActionResult> GetSitesByStaff([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.StaffId == 0)
        {
            return BadRequest("El ID del empleado es requerido");
        }

        var result = await _unitOfWork.SiteStaffRepository.GetSitesByStaff(queryParameters.StaffId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una asignación específica por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID de la asignación</param>
    /// <returns>La asignación si existe</returns>
    [HttpGet("get-assignment-by-id")]
    [SwaggerOperation(Summary = "Obtiene una asignación por ID", Description = "Devuelve una asignación específica basada en su ID.")]
    public async Task<IActionResult> GetAssignmentById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la asignación es requerido");
        }

        var result = await _unitOfWork.SiteStaffRepository.GetSiteStaffById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Asignación con ID {queryParameters.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Asigna un empleado a un sitio
    /// </summary>
    /// <param name="request">Datos de la asignación</param>
    /// <returns>La asignación creada</returns>
    [HttpPost("assign-staff")]
    [SwaggerOperation(Summary = "Asigna un empleado a un sitio", Description = "Crea una nueva asignación de empleado a sitio.")]
    public async Task<IActionResult> AssignStaff([FromBody] SiteStaffRequest request)
    {
        if (request == null)
        {
            return BadRequest("La solicitud de asignación no puede ser nula");
        }

        if (request.SiteId <= 0)
        {
            return BadRequest("El ID del sitio debe ser mayor a 0");
        }

        if (request.StaffId <= 0)
        {
            return BadRequest("El ID del empleado debe ser mayor a 0");
        }

        var result = await _unitOfWork.SiteStaffRepository.AssignStaffToSite(request);

        return Ok(new { Id = result, Message = "Empleado asignado exitosamente" });
    }

    /// <summary>
    /// Actualiza una asignación existente
    /// </summary>
    /// <param name="id">ID de la asignación</param>
    /// <param name="request">Datos de actualización</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-assignment/{id}")]
    [SwaggerOperation(Summary = "Actualiza una asignación", Description = "Actualiza una asignación existente entre empleado y sitio.")]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateSiteStaffRequest request)
    {
        if (id <= 0)
        {
            return BadRequest("El ID de la asignación debe ser mayor a 0");
        }

        if (request == null)
        {
            return BadRequest("La solicitud de actualización no puede ser nula");
        }

        var result = await _unitOfWork.SiteStaffRepository.UpdateSiteStaff(id, request);

        if (result)
        {
            return Ok(new { Message = "Asignación actualizada exitosamente" });
        }

        return BadRequest("Error al actualizar la asignación");
    }

    /// <summary>
    /// Desasigna un empleado de un sitio
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen los IDs del sitio y empleado</param>
    /// <returns>Resultado de la desasignación</returns>
    [HttpDelete("unassign-staff")]
    [SwaggerOperation(Summary = "Desasigna un empleado de un sitio", Description = "Elimina la asignación de un empleado a un sitio específico.")]
    public async Task<IActionResult> UnassignStaff([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.SiteId == null || queryParameters.SiteId == 0 || queryParameters.StaffId == 0)
        {
            return BadRequest("Los IDs del sitio y empleado son requeridos");
        }

        var result = await _unitOfWork.SiteStaffRepository.UnassignStaffFromSite(queryParameters.SiteId ?? 0, queryParameters.StaffId);

        if (result)
        {
            return Ok(new { Message = "Empleado desasignado exitosamente" });
        }

        return BadRequest("Error al desasignar empleado");
    }
}
