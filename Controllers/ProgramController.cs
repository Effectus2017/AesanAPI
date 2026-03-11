using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar programas y sus inscripciones
/// Proporciona endpoints para la gestión completa de programas, incluyendo creación,
/// lectura, actualización y eliminación de programas, así como la gestión de inscripciones a programas.
/// </summary>
[ApiController]
[Route("program")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class ProgramController(IUnitOfWork unitOfWork) : Controller
{
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
        var program = await _unitOfWork.ProgramRepository.GetProgramById(queryParameters.Id);

        if (program == null)
        {
            return NotFound($"Programa con ID {queryParameters.Id} no encontrado");
        }

        return Ok(program);
    }

    /// <summary>
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La lista de programas si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [AllowAnonymous]
    [HttpGet("get-all-programs-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de la base de datos", Description = "Devuelve una lista de todos los programas. Se pueden filtrar por múltiples nombres separados por coma. Accesible sin autenticación para intención de participación.")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var programs = await _unitOfWork.ProgramRepository.GetAllProgramsFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Names,
            queryParameters.Alls,
            queryParameters.IsList
        );
        return Ok(programs);
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
        if (request == null)
        {
            return BadRequest("El programa es requerido");
        }

        var result = await _unitOfWork.ProgramRepository.InsertProgram(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo insertar el programa");
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
        if (request == null)
        {
            return BadRequest("La inscripción de programa es requerida");
        }

        var result = await _unitOfWork.ProgramRepository.InsertProgramInscription(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo insertar la inscripción del programa");
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
        var inscriptions = await _unitOfWork.ProgramRepository.GetAllProgramInscriptions(
            queryParameters.Take,
            queryParameters.Skip,
            agencyId,
            programId
        );
        return Ok(inscriptions);
    }

    /// <summary>
    /// Asigna un evaluador a un programa
    /// </summary>
    /// <param name="request">Request con userId, programId y assignedBy</param>
    /// <returns>True si la asignación fue exitosa</returns>
    [HttpPost("assign-evaluator")]
    [SwaggerOperation(Summary = "Asigna un evaluador a un programa", Description = "Asigna un usuario con rol 'Evaluador' a un programa específico.")]
    public async Task<IActionResult> AssignEvaluatorToProgram([FromBody] AssignEvaluatorToProgramRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserId) || request.ProgramId <= 0)
        {
            return BadRequest("UserId y ProgramId son requeridos");
        }

        // Obtener el usuario que realiza la asignación (desde el token JWT)
        var assignedBy = User?.Identity?.Name ?? request.AssignedBy ?? "System";
        if (string.IsNullOrEmpty(assignedBy))
        {
            assignedBy = "System";
        }

        var result = await _unitOfWork.ProgramRepository.AssignEvaluatorToProgram(request.UserId, request.ProgramId, assignedBy);

        if (result)
        {
            return Ok(new { success = true, message = "Evaluador asignado exitosamente" });
        }

        return BadRequest(new { success = false, message = "No se pudo asignar el evaluador al programa" });
    }

    /// <summary>
    /// Remueve la asignación de un evaluador a un programa
    /// </summary>
    /// <param name="userId">ID del usuario evaluador</param>
    /// <param name="programId">ID del programa</param>
    /// <returns>True si la remoción fue exitosa</returns>
    [HttpDelete("remove-evaluator")]
    [SwaggerOperation(Summary = "Remueve la asignación de un evaluador a un programa", Description = "Remueve la asignación de un evaluador a un programa específico.")]
    public async Task<IActionResult> RemoveEvaluatorFromProgram([FromQuery] string userId, [FromQuery] int programId)
    {
        if (string.IsNullOrEmpty(userId) || programId <= 0)
        {
            return BadRequest("UserId y ProgramId son requeridos");
        }

        var result = await _unitOfWork.ProgramRepository.RemoveEvaluatorFromProgram(userId, programId);

        if (result)
        {
            return Ok(new { success = true, message = "Evaluador removido exitosamente" });
        }

        return BadRequest(new { success = false, message = "No se pudo remover el evaluador del programa" });
    }

    /// <summary>
    /// Obtiene todos los evaluadores asignados a un programa
    /// </summary>
    /// <param name="programId">ID del programa</param>
    /// <returns>Lista de UserIds de los evaluadores</returns>
    [HttpGet("{programId}/evaluators")]
    [SwaggerOperation(Summary = "Obtiene todos los evaluadores asignados a un programa", Description = "Devuelve una lista de UserIds de los evaluadores asignados a un programa específico.")]
    public async Task<IActionResult> GetEvaluatorsByProgramId(int programId)
    {
        if (programId <= 0)
        {
            return BadRequest("ProgramId debe ser mayor a 0");
        }

        var evaluators = await _unitOfWork.ProgramRepository.GetEvaluatorsByProgramId(programId);

        return Ok(new { success = true, data = evaluators, count = evaluators.Count });
    }

}

/// <summary>
/// Request para asignar un evaluador a un programa
/// </summary>
public class AssignEvaluatorToProgramRequest
{
    /// <summary>
    /// ID del usuario evaluador
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// ID del programa
    /// </summary>
    public int ProgramId { get; set; }

    /// <summary>
    /// ID del usuario que realiza la asignación (opcional, se puede obtener del token JWT)
    /// </summary>
    public string? AssignedBy { get; set; }
}
