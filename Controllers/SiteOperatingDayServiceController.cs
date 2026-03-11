using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar servicios de alimentación por día de funcionamiento
/// Proporciona endpoints para la gestión completa de servicios relacionados con días operativos,
/// incluyendo consulta, creación, actualización, eliminación y habilitación/deshabilitación.
/// </summary>
[ApiController]
[Route("site-operating-day-service")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class SiteOperatingDayServiceController(ISiteOperatingDayServiceRepository repository) : Controller
{
    private readonly ISiteOperatingDayServiceRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    /// <summary>
    /// Obtiene todos los servicios de un día de funcionamiento
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del día de funcionamiento</param>
    /// <returns>Lista de servicios del día</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Obtiene servicios de un día de funcionamiento", Description = "Devuelve todos los servicios de alimentación relacionados con un día de funcionamiento específico.")]
    public async Task<IActionResult> GetServicesByOperatingDay([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.OperatingDayId == null || queryParameters.OperatingDayId <= 0)
        {
            return BadRequest("El ID del día de funcionamiento debe ser mayor a 0");
        }

        var services = await _repository.GetServicesByOperatingDay(queryParameters.OperatingDayId.Value);
        return Ok(services);
    }

    /// <summary>
    /// Obtiene un servicio por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del servicio</param>
    /// <returns>Servicio encontrado</returns>
    [HttpGet("get-service-by-id")]
    [SwaggerOperation(Summary = "Obtiene un servicio por ID", Description = "Devuelve la información completa de un servicio de alimentación específico.")]
    public async Task<IActionResult> GetServiceById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id <= 0)
        {
            return BadRequest("El ID del servicio debe ser mayor a 0");
        }

        var service = await _repository.GetServiceById(queryParameters.Id);
        if (service == null)
        {
            return NotFound($"Servicio con ID {queryParameters.Id} no encontrado");
        }

        return Ok(service);
    }

    /// <summary>
    /// Crea un nuevo servicio para un día de funcionamiento
    /// </summary>
    /// <param name="request">Datos del servicio a crear</param>
    /// <returns>ID del servicio creado</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Crea un servicio para un día de funcionamiento", Description = "Crea un nuevo servicio de alimentación relacionado con un día de funcionamiento específico.")]
    public async Task<IActionResult> CreateService([FromBody] SiteOperatingDayServiceRequest request)
    {
        if (request.OperatingDayId == null || request.OperatingDayId <= 0)
        {
            return BadRequest("El ID del día de funcionamiento es requerido");
        }

        var id = await _repository.CreateService(request);

        return Ok(new { Id = id, Message = "Servicio creado exitosamente" });
    }

    /// <summary>
    /// Actualiza un servicio existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del servicio</param>
    /// <param name="request">Datos actualizados del servicio</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("update-service")]
    [SwaggerOperation(Summary = "Actualiza un servicio", Description = "Actualiza la información de un servicio de alimentación existente.")]
    public async Task<IActionResult> UpdateService([FromQuery] QueryParameters queryParameters, [FromBody] SiteOperatingDayServiceUpdateRequest request)
    {
        if (queryParameters.Id <= 0)
        {
            return BadRequest("El ID del servicio debe ser mayor a 0");
        }

        var result = await _repository.UpdateService(queryParameters.Id, request);

        if (result)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudo procesar la solicitud");
        }
    }

    /// <summary>
    /// Elimina un servicio
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del servicio</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("delete-service")]
    [SwaggerOperation(Summary = "Elimina un servicio", Description = "Elimina un servicio de alimentación de un día de funcionamiento.")]
    public async Task<IActionResult> DeleteService([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id <= 0)
        {
            return BadRequest("El ID del servicio debe ser mayor a 0");
        }

        var result = await _repository.DeleteService(queryParameters.Id);

        if (result)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudo procesar la solicitud");
        }
    }

    /// <summary>
    /// Habilita o deshabilita un servicio
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del servicio</param>
    /// <param name="isEnabled">Estado a establecer (true = habilitado, false = deshabilitado)</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("toggle-service")]
    [SwaggerOperation(Summary = "Habilita o deshabilita un servicio", Description = "Cambia el estado de habilitación de un servicio de alimentación.")]
    public async Task<IActionResult> ToggleService([FromQuery] QueryParameters queryParameters, [FromBody] bool isEnabled)
    {
        if (queryParameters.Id <= 0)
        {
            return BadRequest("El ID del servicio debe ser mayor a 0");
        }

        var result = await _repository.ToggleService(queryParameters.Id, isEnabled);

        if (result)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("No se pudo procesar la solicitud");
        }
    }
}
