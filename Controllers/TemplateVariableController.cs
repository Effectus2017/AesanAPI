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
public class TemplateVariableController(TemplateVariableService templateVariableService) : ControllerBase
{
    private readonly TemplateVariableService _templateVariableService = templateVariableService;

    /// <summary>
    /// Obtiene todas las variables disponibles para usar en templates
    /// </summary>
    /// <returns>Lista de variables disponibles</returns>
    [HttpGet("get-all-template-variables")]
    public ActionResult<List<TemplateVariableResponse>> GetAllTemplateVariables()
    {
        var variables = _templateVariableService.GetAllTemplateVariables();
        return Ok(variables);
    }
}
