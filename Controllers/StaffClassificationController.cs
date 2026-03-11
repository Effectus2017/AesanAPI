using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Models.Request;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar clasificaciones de staff
/// Proporciona endpoints para la gestión completa de clasificaciones de staff, incluyendo creación,
/// lectura, actualización y eliminación de clasificaciones de staff.
/// </summary>
[ApiController]
[Route("staff-classification")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class StaffClassificationController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene una clasificación de staff por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La clasificación de staff si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-staff-classification-by-id")]
    [SwaggerOperation(Summary = "Obtiene una clasificación de staff por su ID", Description = "Devuelve una clasificación de staff basada en el ID proporcionado.")]
    public async Task<IActionResult> GetStaffClassificationById([FromQuery] QueryParameters queryParameters)
    {
        var staffClassification = await _unitOfWork.StaffClassificationRepository.GetStaffClassificationById(queryParameters.Id);

        if (staffClassification == null)
        {
            return NotFound("Clasificación de staff no encontrada");
        }

        return Ok(staffClassification);
    }

    /// <summary>
    /// Obtiene todas las clasificaciones de staff
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de clasificaciones de staff</returns>
    [HttpGet("get-all-staff-classifications-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las clasificaciones de staff", Description = "Devuelve una lista paginada de todas las clasificaciones de staff disponibles.")]
    public async Task<IActionResult> GetAllStaffClassifications([FromQuery] QueryParameters queryParameters)
    {
        var staffClassifications = await _unitOfWork.StaffClassificationRepository.GetAllStaffClassificationsFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Names,
            queryParameters.Alls,
            queryParameters.IsList
        );
        return Ok(staffClassifications);
    }

    /// <summary>
    /// Inserta una nueva clasificación de staff
    /// </summary>
    /// <param name="request">La clasificación de staff a insertar</param>
    /// <returns>El resultado de la inserción</returns>
    [HttpPost("insert-staff-classification")]
    [SwaggerOperation(Summary = "Inserta una nueva clasificación de staff", Description = "Crea una nueva clasificación de staff en la base de datos.")]
    public async Task<IActionResult> InsertStaffClassification([FromBody] StaffClassificationRequest request)
    {
        if (request == null)
        {
            return BadRequest("La clasificación de staff es requerida");
        }

        var result = await _unitOfWork.StaffClassificationRepository.InsertStaffClassification(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo insertar la clasificación de staff");
    }

    /// <summary>
    /// Actualiza una clasificación de staff existente
    /// </summary>
    /// <param name="request">La clasificación de staff a actualizar</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-staff-classification")]
    [SwaggerOperation(Summary = "Actualiza una clasificación de staff existente", Description = "Actualiza una clasificación de staff existente en la base de datos.")]
    public async Task<IActionResult> UpdateStaffClassification([FromBody] StaffClassificationRequest request)
    {
        if (request == null)
        {
            return BadRequest("La clasificación de staff es requerida");
        }

        var result = await _unitOfWork.StaffClassificationRepository.UpdateStaffClassification(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo actualizar la clasificación de staff");
    }

    /// <summary>
    /// Elimina una clasificación de staff (baja lógica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la eliminación</returns>
    [HttpDelete("delete-staff-classification")]
    [SwaggerOperation(Summary = "Elimina una clasificación de staff", Description = "Realiza una baja lógica de una clasificación de staff existente.")]
    public async Task<IActionResult> DeleteStaffClassification([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.StaffClassificationRepository.DeleteStaffClassification(queryParameters.Id);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo eliminar la clasificación de staff");
    }
}
