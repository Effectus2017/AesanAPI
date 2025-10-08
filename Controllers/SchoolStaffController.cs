using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar las asignaciones de empleados a sitios
/// Proporciona endpoints para asignar, desasignar y consultar empleados en sitios
/// </summary>
[Route("school-staff")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SchoolStaffController(ILogger<SchoolStaffController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<SchoolStaffController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene todos los empleados asignados a un sitio específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del sitio</param>
    /// <returns>Lista de empleados asignados al sitio</returns>
    [HttpGet("get-staff-by-school")]
    [SwaggerOperation(Summary = "Obtiene empleados asignados a un sitio", Description = "Devuelve todos los empleados asignados a un sitio específico.")]
    public async Task<IActionResult> GetStaffBySchool([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.SchoolId == null || queryParameters.SchoolId == 0)
                {
                    return BadRequest("El ID del sitio es requerido");
                }

                _logger.LogInformation("Obteniendo empleados asignados al sitio {SchoolId}", queryParameters.SchoolId);

                var result = await _unitOfWork.SchoolStaffRepository.GetStaffBySchool(queryParameters.SchoolId ?? 0);
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados del sitio {SchoolId}", queryParameters.SchoolId);
            return StatusCode(500, "Error al obtener empleados del sitio");
        }
    }

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del empleado</param>
    /// <returns>Lista de sitios asignados al empleado</returns>
    [HttpGet("get-schools-by-staff")]
    [SwaggerOperation(Summary = "Obtiene sitios asignados a un empleado", Description = "Devuelve todos los sitios asignados a un empleado específico.")]
    public async Task<IActionResult> GetSchoolsByStaff([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.StaffId == 0)
                {
                    return BadRequest("El ID del empleado es requerido");
                }

                _logger.LogInformation("Obteniendo sitios asignados al empleado {StaffId}", queryParameters.StaffId);

                var result = await _unitOfWork.SchoolStaffRepository.GetSchoolsByStaff(queryParameters.StaffId);
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sitios del empleado {StaffId}", queryParameters.StaffId);
            return StatusCode(500, "Error al obtener sitios del empleado");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID de la asignación es requerido");
                }

                _logger.LogInformation("Obteniendo asignación con ID {Id}", queryParameters.Id);

                var result = await _unitOfWork.SchoolStaffRepository.GetSchoolStaffById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Asignación con ID {queryParameters.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la asignación con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error al obtener la asignación");
        }
    }

    /// <summary>
    /// Asigna un empleado a un sitio
    /// </summary>
    /// <param name="request">Datos de la asignación</param>
    /// <returns>La asignación creada</returns>
    [HttpPost("assign-staff")]
    [SwaggerOperation(Summary = "Asigna un empleado a un sitio", Description = "Crea una nueva asignación de empleado a sitio.")]
    public async Task<IActionResult> AssignStaff([FromBody] SchoolStaffRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Asignando empleado {StaffId} al sitio {SchoolId}", request.StaffId, request.SchoolId);

                var result = await _unitOfWork.SchoolStaffRepository.AssignStaffToSchool(request);

                return Ok(new { Id = result, Message = "Empleado asignado exitosamente" });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar empleado {StaffId} al sitio {SchoolId}", request.StaffId, request.SchoolId);
            return StatusCode(500, "Error al asignar empleado al sitio");
        }
    }

    /// <summary>
    /// Actualiza una asignación existente
    /// </summary>
    /// <param name="id">ID de la asignación</param>
    /// <param name="request">Datos de actualización</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-assignment/{id}")]
    [SwaggerOperation(Summary = "Actualiza una asignación", Description = "Actualiza una asignación existente entre empleado y sitio.")]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateSchoolStaffRequest request)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El ID de la asignación debe ser mayor a 0");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Actualizando asignación con ID {Id}", id);

                var result = await _unitOfWork.SchoolStaffRepository.UpdateSchoolStaff(id, request);

                if (result)
                {
                    return Ok(new { Message = "Asignación actualizada exitosamente" });
                }

                return BadRequest("Error al actualizar la asignación");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación con ID {Id}", id);
            return StatusCode(500, "Error al actualizar la asignación");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.SchoolId == null || queryParameters.SchoolId == 0 || queryParameters.StaffId == 0)
                {
                    return BadRequest("Los IDs del sitio y empleado son requeridos");
                }

                _logger.LogInformation("Desasignando empleado {StaffId} del sitio {SchoolId}", queryParameters.StaffId, queryParameters.SchoolId);

                var result = await _unitOfWork.SchoolStaffRepository.UnassignStaffFromSchool(queryParameters.SchoolId ?? 0, queryParameters.StaffId);

                if (result)
                {
                    return Ok(new { Message = "Empleado desasignado exitosamente" });
                }

                return BadRequest("Error al desasignar empleado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desasignar empleado {StaffId} del sitio {SchoolId}", queryParameters.StaffId, queryParameters.SchoolId);
            return StatusCode(500, "Error al desasignar empleado del sitio");
        }
    }
}
