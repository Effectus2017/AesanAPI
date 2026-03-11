using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Api.Models.Errors;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de centro.
/// Proporciona endpoints para la gestión completa de tipos de centro, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[ApiController]
[Route("center-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class CenterTypeController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un tipo de centro por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Tipo de centro</returns>
    [HttpGet("get-center-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de centro por su ID", Description = "Devuelve un tipo de centro basado en el ID proporcionado.")]
    public async Task<IActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.CenterTypeRepository.GetCenterTypeById(queryParameters.Id);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de centro
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Lista de tipos de centro</returns>
    [HttpGet("get-all-center-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de centro", Description = "Devuelve una lista de tipos de centro.")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.CenterTypeRepository.GetAllCenterTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name ?? string.Empty, queryParameters.Alls, queryParameters.IsList);
        return Ok(result);
    }

    /// <summary>
    /// Inserta un nuevo tipo de centro
    /// </summary>
    /// <param name="request">Tipo de centro</param>
    /// <returns>ID del tipo de centro creado</returns>
    [HttpPost("insert-center-type")]
    [SwaggerOperation(Summary = "Inserta un nuevo tipo de centro", Description = "Crea un nuevo tipo de centro en la base de datos.")]
    public async Task<IActionResult> Insert([FromBody] CenterTypeRequest request)
    {
        var newId = await _unitOfWork.CenterTypeRepository.InsertCenterType(request);
        return Ok(new { id = newId, message = "Tipo de centro creado exitosamente" });
    }

    /// <summary>
    /// Actualiza un tipo de centro existente
    /// </summary>
    /// <param name="request">Tipo de centro</param>
    /// <returns>Tipo de centro actualizado</returns>
    [HttpPut("update-center-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de centro existente", Description = "Actualiza los datos de un tipo de centro existente.")]
    public async Task<IActionResult> Update([FromBody] DTOCenterType request)
    {
        var result = await _unitOfWork.CenterTypeRepository.UpdateCenterType(request);
        return NoContent();
    }

    /// <summary>
    /// Elimina un tipo de centro
    /// </summary>
    /// <param name="id">ID del tipo de centro</param>
    /// <returns>Tipo de centro eliminado</returns>
    [HttpDelete("delete-center-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de centro", Description = "Elimina un tipo de centro de la base de datos.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.CenterTypeRepository.DeleteCenterType(queryParameters.Id);
        return NoContent();
    }

    /// <summary>
    /// Obtiene los tipos de centro válidos para un programa específico
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del programa</param>
    /// <returns>Lista de tipos de centro válidos para el programa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-center-types-by-program")]
    [SwaggerOperation(Summary = "Obtiene tipos de centro por programa", Description = "Devuelve los tipos de centro válidos para un programa específico.")]
    public async Task<ActionResult> GetCenterTypesByProgram([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.ProgramId == 0 || !queryParameters.ProgramId.HasValue)
        {
            return BadRequest("El ID del programa es requerido");
        }

        var result = await _unitOfWork.CenterTypeRepository.GetCenterTypesByProgram(queryParameters.ProgramId.Value);

        return Ok(result);
    }
}
