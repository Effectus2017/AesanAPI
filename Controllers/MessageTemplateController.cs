using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los templates de mensajes.
/// Proporciona endpoints para la gestión completa de templates de mensajes, incluyendo creación,
/// lectura, actualización de templates.
/// </summary>
[ApiController]
[Route("message-template")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class MessageTemplateController(IMessageTemplateRepository messageTemplateRepository) : ControllerBase
{
    private readonly IMessageTemplateRepository _messageTemplateRepository = messageTemplateRepository;

    /// <summary>
    /// Obtiene un template de mensaje por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Template de mensaje</returns>
    [HttpGet("get-message-template-by-id")]
    [SwaggerOperation(Summary = "Obtiene un template de mensaje por su ID", Description = "Devuelve un template de mensaje basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID del template de mensaje es requerido");
        }

        var result = await _messageTemplateRepository.GetMessageTemplateById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Template de mensaje con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los templates de mensaje
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de templates de mensaje</returns>
    [HttpGet("get-all-message-templates")]
    [SwaggerOperation(Summary = "Obtiene todos los templates de mensaje", Description = "Devuelve una lista de templates de mensaje.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _messageTemplateRepository.GetAllMessageTemplates(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.TemplateKey,
            queryParameters.Description, // Este parámetro ahora se mapea a Purpose en el stored procedure
            queryParameters.Alls);

        if (result == null)
        {
            return NotFound("No se encontraron templates de mensaje");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un template de mensaje por su clave
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen la clave del template</param>
    /// <returns>Template de mensaje</returns>
    [HttpGet("get-message-template-by-key")]
    [SwaggerOperation(Summary = "Obtiene un template de mensaje por su clave", Description = "Devuelve un template de mensaje basado en la clave proporcionada.")]
    public async Task<ActionResult> GetByKey([FromQuery] QueryParameters queryParameters)
    {
        if (string.IsNullOrEmpty(queryParameters.TemplateKey))
        {
            return BadRequest("La clave del template es requerida");
        }

        var result = await _messageTemplateRepository.GetMessageTemplateByKey(queryParameters.TemplateKey);

        if (result == null)
        {
            return NotFound($"Template de mensaje con clave {queryParameters.TemplateKey} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo template de mensaje
    /// </summary>
    /// <param name="request">El template de mensaje a crear</param>
    /// <returns>El template de mensaje creado</returns>
    [HttpPost("insert-message-template")]
    [SwaggerOperation(Summary = "Crea un nuevo template de mensaje", Description = "Crea un nuevo template de mensaje.")]
    public async Task<ActionResult> Insert([FromBody] MessageTemplateRequest request)
    {
        if (request == null)
        {
            return BadRequest("El template de mensaje es requerido");
        }

        var result = await _messageTemplateRepository.InsertMessageTemplate(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear el template de mensaje");
    }

    /// <summary>
    /// Actualiza un template de mensaje existente
    /// </summary>
    /// <param name="request">El template de mensaje a actualizar</param>
    /// <returns>El template de mensaje actualizado</returns>
    [HttpPut("update-message-template")]
    [SwaggerOperation(Summary = "Actualiza un template de mensaje existente", Description = "Actualiza los datos de un template de mensaje existente.")]
    public async Task<IActionResult> Update([FromBody] MessageTemplateRequest request)
    {
        if (request == null || request.Id == null || request.Id == 0)
        {
            return BadRequest("El ID del template de mensaje es requerido");
        }

        var result = await _messageTemplateRepository.UpdateMessageTemplate(request);

        if (!result)
        {
            return NotFound($"Template de mensaje con ID {request.Id} no encontrado");
        }

        return Ok(result);
    }
}
