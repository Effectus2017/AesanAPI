using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controller para gestionar las variables disponibles en los templates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class TemplateVariableController : ControllerBase
{
    private readonly TemplateVariableService _templateVariableService;
    private readonly ILogger<TemplateVariableController> _logger;

    public TemplateVariableController(
        TemplateVariableService templateVariableService,
        ILogger<TemplateVariableController> logger)
    {
        _templateVariableService = templateVariableService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las variables disponibles para usar en templates
    /// </summary>
    /// <returns>Lista de variables disponibles</returns>
    [HttpGet("get-all-template-variables")]
    public ActionResult<List<TemplateVariableResponse>> GetAllTemplateVariables()
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las variables de templates disponibles");
            var variables = _templateVariableService.GetAllTemplateVariables();
            return Ok(variables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las variables de templates");
            return StatusCode(500, "Error interno del servidor al obtener las variables de templates");
        }
    }
}

