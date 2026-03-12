using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar el calendario de citas de las agencias
/// </summary>
[ApiController]
[Route("agency-calendar")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class AgencyCalendarController(IAgencyCalendarRepository agencyCalendarRepository) : Controller
{
    private readonly IAgencyCalendarRepository _agencyCalendarRepository = agencyCalendarRepository ?? throw new ArgumentNullException(nameof(agencyCalendarRepository));

    [HttpGet("get-agency-appointments")]
    [SwaggerOperation(Summary = "Obtiene las citas de una agencia", Description = "Devuelve las citas para una agencia específica. Filtra opcionalmente por mes y año.")]
    public async Task<IActionResult> GetAppointments([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.AgencyId <= 0)
        {
            return BadRequest("El ID de la agencia debe ser mayor a 0");
        }

        var result = await _agencyCalendarRepository.GetAppointments(
            queryParameters.AgencyId,
            queryParameters.Month,
            queryParameters.Year
        );
        return Ok(result);
    }

    [HttpPost("create-appointment")]
    [SwaggerOperation(Summary = "Crea una nueva cita", Description = "Crea una nueva cita para una agencia específica.")]
    public async Task<IActionResult> CreateAppointment([FromBody] AgencyAppointmentRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        var newId = await _agencyCalendarRepository.CreateAppointment(request, userId);

        if (newId.HasValue)
        {
            return Ok(new { id = newId.Value, success = true });
        }

        return BadRequest("No se pudo procesar la solicitud");
    }

    [HttpPut("update-appointment")]
    [SwaggerOperation(Summary = "Actualiza una cita", Description = "Actualiza una cita existente.")]
    public async Task<IActionResult> UpdateAppointment([FromBody] AgencyAppointmentRequest request)
    {
        if (!request.Id.HasValue || request.Id.Value <= 0)
        {
            return BadRequest("El ID de la cita es requerido para actualizar");
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        var result = await _agencyCalendarRepository.UpdateAppointment(request.Id.Value, request, userId);

        if (result)
        {
            return Ok(true);
        }

        return BadRequest("No se pudo procesar la solicitud");
    }

    [HttpDelete("delete-appointment/{id}")]
    [SwaggerOperation(Summary = "Elimina una cita", Description = "Elimina de forma lógica una cita específica.")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        if (id <= 0)
        {
            return BadRequest("El ID debe ser mayor a 0");
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        var result = await _agencyCalendarRepository.DeleteAppointment(id, userId);

        if (result)
        {
            return Ok(true);
        }

        return BadRequest("No se pudo eliminar la cita");
    }
}
