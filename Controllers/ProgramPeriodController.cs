using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar períodos de programas
/// </summary>
[ApiController]
[Route("program-period")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class ProgramPeriodController(IProgramPeriodService service) : Controller
{
    private readonly IProgramPeriodService _service = service ?? throw new ArgumentNullException(nameof(service));

    /// <summary>
    /// Obtiene todos los períodos de un programa
    /// </summary>
    [HttpGet("get-by-program-id")]
    [SwaggerOperation(Summary = "Obtiene todos los períodos de un programa")]
    public async Task<IActionResult> GetProgramPeriodsByProgramId([FromQuery] int programId)
    {
        if (programId <= 0)
        {
            return BadRequest("El ID del programa debe ser mayor que 0");
        }

        var periods = await _service.GetProgramPeriodsByProgramId(programId);
        return Ok(periods);
    }

    /// <summary>
    /// Obtiene un período específico de un programa por año
    /// </summary>
    [HttpGet("get-by-program-id-and-year")]
    [SwaggerOperation(Summary = "Obtiene un período específico de un programa por año")]
    public async Task<IActionResult> GetProgramPeriodByProgramIdAndYear([FromQuery] int programId, [FromQuery] int year)
    {
        if (programId <= 0)
        {
            return BadRequest("El ID del programa debe ser mayor que 0");
        }

        if (year < 2000 || year > 2100)
        {
            return BadRequest("El año debe estar entre 2000 y 2100");
        }

        var period = await _service.GetProgramPeriodByProgramIdAndYear(programId, year);

        if (period == null)
        {
            return NotFound($"No se encontró un período para el programa {programId} en el año {year}");
        }

        return Ok(period);
    }

    /// <summary>
    /// Inserta un nuevo período de programa
    /// </summary>
    [HttpPost("insert")]
    [SwaggerOperation(Summary = "Inserta un nuevo período de programa")]
    public async Task<IActionResult> InsertProgramPeriod([FromBody] ProgramPeriodRequest request)
    {
        var result = await _service.InsertProgramPeriod(request);
        return Ok(result);
    }

    /// <summary>
    /// Actualiza un período de programa existente
    /// </summary>
    [HttpPut("update")]
    [SwaggerOperation(Summary = "Actualiza un período de programa existente")]
    public async Task<IActionResult> UpdateProgramPeriod([FromBody] ProgramPeriodRequest request)
    {
        if (!request.Id.HasValue)
        {
            return BadRequest("El ID del período es requerido para actualizar");
        }

        var result = await _service.UpdateProgramPeriod(request);

        if (!result)
        {
            return NotFound($"No se encontró el período con ID {request.Id}");
        }

        return Ok(new { success = true });
    }

    /// <summary>
    /// Calcula automáticamente las fechas de inicio y fin para un programa y año
    /// </summary>
    [HttpGet("calculate-dates")]
    [SwaggerOperation(Summary = "Calcula automáticamente las fechas de inicio y fin para un programa y año")]
    public IActionResult CalculateProgramPeriodDates([FromQuery] int programId, [FromQuery] int year)
    {
        if (programId <= 0)
        {
            return BadRequest("El ID del programa debe ser mayor que 0");
        }

        if (year < 2000 || year > 2100)
        {
            return BadRequest("El año debe estar entre 2000 y 2100");
        }

        var (startDate, endDate) = _service.CalculateProgramPeriodDates(programId, year);

        return Ok(new
        {
            programId,
            year,
            startDate,
            endDate
        });
    }
}
