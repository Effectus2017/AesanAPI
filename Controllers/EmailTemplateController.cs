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
/// Controlador que maneja todas las operaciones relacionadas con los templates de email.
/// Proporciona endpoints para la gestión completa de templates de email, incluyendo creación,
/// lectura, actualización de templates.
/// </summary>
[ApiController]
[Route("email-template")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class EmailTemplateController(IEmailTemplateRepository emailTemplateRepository, ILogger<EmailTemplateController> logger) : ControllerBase
{
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly ILogger<EmailTemplateController> _logger = logger;

    /// <summary>
    /// Obtiene un template de email por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Template de email</returns>
    [HttpGet("get-email-template-by-id")]
    [SwaggerOperation(Summary = "Obtiene un template de email por su ID", Description = "Devuelve un template de email basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo template de email por ID: {Id}", queryParameters.Id);

            if (queryParameters.Id == 0)
            {
                return BadRequest("El ID del template de email es requerido");
            }

            var result = await _emailTemplateRepository.GetEmailTemplateById(queryParameters.Id);

            if (result == null)
            {
                return NotFound($"Template de email con ID {queryParameters.Id} no encontrado");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el template de email con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el template de email");
        }
    }

    /// <summary>
    /// Obtiene todos los templates de email
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de templates de email</returns>
    [HttpGet("get-all-email-templates")]
    [SwaggerOperation(Summary = "Obtiene todos los templates de email", Description = "Devuelve una lista de templates de email.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los templates de email");

            var result = await _emailTemplateRepository.GetAllEmailTemplates(
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.TemplateKey,
                queryParameters.Description,
                queryParameters.Alls);

            if (result == null)
            {
                return NotFound("No se encontraron templates de email");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los templates de email");
            return StatusCode(500, "Error interno del servidor al obtener los templates de email");
        }
    }

    /// <summary>
    /// Obtiene un template de email por su clave
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen la clave del template</param>
    /// <returns>Template de email</returns>
    [HttpGet("get-email-template-by-key")]
    [SwaggerOperation(Summary = "Obtiene un template de email por su clave", Description = "Devuelve un template de email basado en la clave proporcionada.")]
    public async Task<ActionResult> GetByKey([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (string.IsNullOrEmpty(queryParameters.TemplateKey))
            {
                return BadRequest("La clave del template es requerida");
            }

            var result = await _emailTemplateRepository.GetEmailTemplateByKey(queryParameters.TemplateKey);

            if (result == null)
            {
                return NotFound($"Template de email con clave {queryParameters.TemplateKey} no encontrado");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el template de email con clave {TemplateKey}", queryParameters.TemplateKey);
            return StatusCode(500, "Error interno del servidor al obtener el template de email");
        }
    }

    /// <summary>
    /// Crea un nuevo template de email
    /// </summary>
    /// <param name="request">El template de email a crear</param>
    /// <returns>El template de email creado</returns>
    [HttpPost("insert-email-template")]
    [SwaggerOperation(Summary = "Crea un nuevo template de email", Description = "Crea un nuevo template de email.")]
    public async Task<ActionResult> Insert([FromBody] EmailTemplateRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("El template de email es requerido");
            }

            var result = await _emailTemplateRepository.InsertEmailTemplate(request);

            if (result)
            {
                _logger.LogInformation("Template de email creado con clave: {TemplateKey}", request.TemplateKey);
                return Ok(result);
            }

            _logger.LogWarning("No se pudo crear el template de email");
            return BadRequest("No se pudo crear el template de email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el template de email");
            return StatusCode(500, "Error interno del servidor al crear el template de email");
        }
    }

    /// <summary>
    /// Actualiza un template de email existente
    /// </summary>
    /// <param name="request">El template de email a actualizar</param>
    /// <returns>El template de email actualizado</returns>
    [HttpPut("update-email-template")]
    [SwaggerOperation(Summary = "Actualiza un template de email existente", Description = "Actualiza los datos de un template de email existente.")]
    public async Task<IActionResult> Update([FromBody] EmailTemplateRequest request)
    {
        try
        {
            if (request == null || request.Id == null || request.Id == 0)
            {
                return BadRequest("El ID del template de email es requerido");
            }

            var result = await _emailTemplateRepository.UpdateEmailTemplate(request);

            if (!result)
            {
                _logger.LogWarning("Template de email con ID {Id} no encontrado", request.Id);
                return NotFound($"Template de email con ID {request.Id} no encontrado");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el template de email con ID {Id}", request?.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el template de email");
        }
    }
}

