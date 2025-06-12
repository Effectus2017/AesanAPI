using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de centro.
/// Proporciona endpoints para la gestión completa de tipos de centro, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[Route("center-type")]
[ApiController]
public class CenterTypeController(ILogger<CenterTypeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<CenterTypeController> _logger = logger;
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
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo tipo de centro por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del tipo de centro es requerido");
                }

                var result = await _unitOfWork.CenterTypeRepository.GetCenterTypeById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Tipo de centro con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de centro");
            return StatusCode(500, "Error al obtener el tipo de centro");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.CenterTypeRepository.GetAllCenterTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No se encontraron tipos de centro");
                }

                return Ok(result);
            }

            _logger.LogError("Error al obtener los tipos de centro");
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de centro");
            return StatusCode(500, "Error al obtener los tipos de centro");
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de centro
    /// </summary>
    /// <param name="request">Tipo de centro</param>
    /// <returns>ID del tipo de centro</returns>
    [HttpPost("insert-center-type")]
    [SwaggerOperation(Summary = "Inserta un nuevo tipo de centro", Description = "Crea un nuevo tipo de centro en la base de datos.")]
    public async Task<IActionResult> Insert([FromBody] CenterTypeRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.CenterTypeRepository.InsertCenterType(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo insertar el tipo de centro");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de centro");
            return StatusCode(500, "Error al insertar el tipo de centro");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.CenterTypeRepository.UpdateCenterType(request);

                if (!result)
                {
                    return NotFound($"Tipo de centro con ID {request.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de centro");
            return StatusCode(500, "Error al actualizar el tipo de centro");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.CenterTypeRepository.DeleteCenterType(queryParameters.Id);

                if (!result)
                {
                    return NotFound($"Tipo de centro con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de centro");
            return StatusCode(500, "Error al eliminar el tipo de centro");
        }
    }

}