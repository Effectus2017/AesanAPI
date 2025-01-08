using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("program")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPost("insert-program")]
    [SwaggerOperation(Summary = "Inserta un nuevo programa", Description = "Crea un nuevo programa en la base de datos.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> InsertProgram([FromBody] ProgramRequest programRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var programId = await _unitOfWork.ProgramRepository.InsertProgram(programRequest);
                return Ok(new { id = programId });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el programa");
            return StatusCode(500, "Error al insertar el programa");
        }
    }

    [HttpPost("insert-program-inscription")]
    [SwaggerOperation(Summary = "Inserta una nueva inscripción de programa", Description = "Crea una nueva inscripción de programa en la base de datos.")]
    // #if !DEBUG
    //     [Authorize]
    // #endif
    public async Task<IActionResult> InsertProgramInscription([FromBody] ProgramInscriptionRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var inscriptionId = await _unitOfWork.ProgramRepository.InsertProgramInscription(request);
                return Ok(new { id = inscriptionId });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la inscripción del programa");
            return StatusCode(500, "Error al insertar la inscripción del programa");
        }
    }

    [HttpGet("get-all-program-inscriptions")]
    [SwaggerOperation(Summary = "Obtiene todas las inscripciones a programas", Description = "Devuelve una lista paginada de inscripciones a programas.")]
    public async Task<IActionResult> GetAllProgramInscriptions([FromQuery] QueryParameters queryParameters, [FromQuery] int? agencyId = null, [FromQuery] int? programId = null)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var inscriptions = await _unitOfWork.ProgramRepository.GetAllProgramInscriptions(
                    queryParameters.Take,
                    queryParameters.Skip,
                    agencyId,
                    programId
                );
                return Ok(inscriptions);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las inscripciones de programas");
            return StatusCode(500, "Error al obtener las inscripciones de programas");
        }
    }

}
