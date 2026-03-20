using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Asignaciones de empleados a escuelas (SchoolStaff).
/// </summary>
[ApiController]
[Route("school-staff")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class SchoolStaffController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>Obtiene empleados asignados a una escuela.</summary>
    [HttpGet("get-staff-by-school")]
    [SwaggerOperation(Summary = "Empleados por escuela", Description = "Lista de empleados asignados a una escuela.")]
    public async Task<IActionResult> GetStaffBySchool([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.SchoolId == null || queryParameters.SchoolId == 0)
        {
            return BadRequest("El ID de la escuela es requerido");
        }

        var result = await _unitOfWork.SchoolStaffRepository.GetStaffBySchool(queryParameters.SchoolId ?? 0);
        return Ok(result);
    }

    /// <summary>Obtiene escuelas asignadas a un empleado.</summary>
    [HttpGet("get-schools-by-staff")]
    [SwaggerOperation(Summary = "Escuelas por empleado", Description = "Lista de escuelas asignadas a un empleado.")]
    public async Task<IActionResult> GetSchoolsByStaff([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.StaffId == 0)
        {
            return BadRequest("El ID del empleado es requerido");
        }

        var result = await _unitOfWork.SchoolStaffRepository.GetSchoolsByStaff(queryParameters.StaffId);
        return Ok(result);
    }

    /// <summary>Obtiene una asignación por su ID.</summary>
    [HttpGet("get-assignment-by-id")]
    [SwaggerOperation(Summary = "Asignación por ID", Description = "Detalle de una asignación SchoolStaff.")]
    public async Task<IActionResult> GetAssignmentById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la asignación es requerido");
        }

        var result = await _unitOfWork.SchoolStaffRepository.GetSchoolStaffById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Asignación con ID {queryParameters.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>Crea una asignación empleado–escuela.</summary>
    [HttpPost("assign-staff")]
    [SwaggerOperation(Summary = "Asignar empleado a escuela", Description = "Crea una asignación SchoolStaff.")]
    public async Task<IActionResult> AssignStaff([FromBody] SchoolStaffRequest request)
    {
        if (request == null)
        {
            return BadRequest("La solicitud de asignación no puede ser nula");
        }

        if (request.SchoolId <= 0)
        {
            return BadRequest("El ID de la escuela debe ser mayor a 0");
        }

        if (request.StaffId <= 0)
        {
            return BadRequest("El ID del empleado debe ser mayor a 0");
        }

        var result = await _unitOfWork.SchoolStaffRepository.AssignStaffToSchool(request);

        return Ok(new { Id = result, Message = "Empleado asignado exitosamente" });
    }

    /// <summary>Actualiza una asignación existente.</summary>
    [HttpPut("update-assignment/{id}")]
    [SwaggerOperation(Summary = "Actualizar asignación", Description = "Actualiza datos de una asignación SchoolStaff.")]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateSchoolStaffRequest request)
    {
        if (id <= 0)
        {
            return BadRequest("El ID de la asignación debe ser mayor a 0");
        }

        if (request == null)
        {
            return BadRequest("La solicitud de actualización no puede ser nula");
        }

        var result = await _unitOfWork.SchoolStaffRepository.UpdateSchoolStaff(id, request);

        if (result)
        {
            return Ok(new { Message = "Asignación actualizada exitosamente" });
        }

        return BadRequest("Error al actualizar la asignación");
    }

    /// <summary>Desasigna un empleado de una escuela.</summary>
    [HttpDelete("unassign-staff")]
    [SwaggerOperation(Summary = "Desasignar empleado", Description = "Desactiva la asignación empleado–escuela.")]
    public async Task<IActionResult> UnassignStaff([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.SchoolId == null || queryParameters.SchoolId == 0 || queryParameters.StaffId == 0)
        {
            return BadRequest("Los IDs de escuela y empleado son requeridos");
        }

        var result = await _unitOfWork.SchoolStaffRepository.UnassignStaffFromSchool(queryParameters.SchoolId ?? 0, queryParameters.StaffId);

        if (result)
        {
            return Ok(new { Message = "Empleado desasignado exitosamente" });
        }

        return BadRequest("Error al desasignar empleado");
    }
}
