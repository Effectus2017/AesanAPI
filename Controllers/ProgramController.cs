using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar programas y sus inscripciones
/// Proporciona endpoints para la gestión completa de programas, incluyendo creación,
/// lectura, actualización y eliminación de programas, así como la gestión de inscripciones a programas.
/// </summary>
[Route("program")]
[ApiController]
public class ProgramController(ILogger<ProgramController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<ProgramController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un programa por su ID
    /// </summary>
    /// <param name="id">ID del programa a obtener</param>
    /// <returns>El programa si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-program-by-id")]
    [SwaggerOperation(Summary = "Obtiene un programa por su ID", Description = "Devuelve un programa basado en el ID proporcionado.")]
    public async Task<IActionResult> GetProgramById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo programa por ID: {Id}", queryParameters.Id);

                var program = await _unitOfWork.ProgramRepository.GetProgramById(queryParameters.Id);

                if (program == null)
                {
                    return NotFound($"Programa con ID {queryParameters.Id} no encontrado");
                }

                return Ok(program);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el programa con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el programa");
        }
    }

    /// <summary>
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La lista de programas si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-programs-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de la base de datos", Description = "Devuelve una lista de todos los programas. Se pueden filtrar por múltiples nombres separados por coma.")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todos los programas");

                var programs = await _unitOfWork.ProgramRepository.GetAllProgramsFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Names,
                    queryParameters.Alls,
                    queryParameters.IsList
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

    /// <summary>
    /// Inserta un nuevo programa
    /// </summary>
    /// <param name="programRequest">El programa a insertar</param>
    /// <returns>El ID del programa insertado</returns>
    [HttpPost("insert-program")]
    [SwaggerOperation(Summary = "Inserta un nuevo programa", Description = "Crea un nuevo programa en la base de datos.")]
    public async Task<IActionResult> Insert([FromBody] ProgramRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El programa es requerido");
                }

                var result = await _unitOfWork.ProgramRepository.InsertProgram(request);

                if (result)
                {
                    _logger.LogInformation("Programa insertado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo insertar el programa");
                return BadRequest("No se pudo insertar el programa");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el programa");
            return StatusCode(500, "Error al insertar el programa");
        }
    }

    /// <summary>
    /// Inserta una nueva inscripción de programa
    /// </summary>
    /// <param name="request">La inscripción de programa a insertar</param>
    /// <returns>La inscripción de programa insertada</returns>
    [HttpPost("insert-program-inscription")]
    [SwaggerOperation(Summary = "Inserta una nueva inscripción de programa", Description = "Crea una nueva inscripción de programa en la base de datos.")]
    public async Task<IActionResult> InsertProgramInscription([FromBody] ProgramInscriptionRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("La inscripción de programa es requerida");
                }

                var result = await _unitOfWork.ProgramRepository.InsertProgramInscription(request);

                if (result)
                {
                    _logger.LogInformation("Inscripción de programa insertada");
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo insertar la inscripción del programa");
                return BadRequest("No se pudo insertar la inscripción del programa");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la inscripción del programa");
            return StatusCode(500, "Error al insertar la inscripción del programa");
        }
    }

    /// <summary>
    /// Obtiene todas las inscripciones a programas
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <param name="agencyId">El ID de la agencia</param>
    /// <param name="programId">El ID del programa</param>
    /// <returns>La lista de inscripciones a programas si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
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
