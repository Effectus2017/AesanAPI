using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("program")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProgramController(ILogger<ProgramController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<ProgramController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    [HttpGet("get-all-programs-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de la base de datos", Description = "Devuelve una lista de todos los programas.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetAllPrograms([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var programs = await _unitOfWork.ProgramRepository.GetAllProgramsFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Name,
                    queryParameters.Alls
                );
                return Ok(programs);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los programas");
            return StatusCode(500, "Error al obtener los programas");
        }
    }

    [HttpGet("get-program-by-id")]
    [SwaggerOperation(Summary = "Obtiene un programa por su ID", Description = "Devuelve un programa basado en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetProgramById([FromQuery] int id)
    {
        try
        {
            var program = await _unitOfWork.ProgramRepository.GetProgramById(id);
            if (program == null)
            {
                return NotFound($"Programa con ID {id} no encontrado");
            }
            return Ok(program);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el programa");
            return StatusCode(500, "Error al obtener el programa");
        }
    }
}
