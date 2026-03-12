using Api.Interfaces;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;


/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los archivos de agencias.
/// Proporciona endpoints para subir, obtener, actualizar y eliminar archivos asociados a agencias.
/// </summary>
[ApiController]
[Route("agency-files")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class AgencyFilesController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un archivo específico por su ID
    /// </summary>
    /// <param name="id">ID del archivo</param>
    /// <returns>Información del archivo</returns>
    [HttpGet("get-agency-file-by-id")]
    [SwaggerOperation(Summary = "Obtiene un archivo por su ID", Description = "Devuelve la información de un archivo específico.")]
    public async Task<IActionResult> GetFile([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.AgencyFilesRepository.GetAgencyFileById(queryParameters.Id);

        return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
    }

    /// <summary>
    /// Obtiene los archivos asociados a una agencia específica
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="documentType">Tipo de documento (opcional)</param>
    /// <returns>Lista paginada de archivos de la agencia</returns>
    [HttpGet("get-agency-files-from-db")]
    [SwaggerOperation(Summary = "Obtiene los archivos de una agencia", Description = "Devuelve una lista paginada de archivos asociados a una agencia específica.")]
    public async Task<IActionResult> GetAgencyFiles([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.AgencyFilesRepository.GetAgencyFiles(queryParameters.AgencyId, queryParameters.Take, queryParameters.Skip, queryParameters.Alls, queryParameters.Name, queryParameters.DocumentType);
        return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
    }


    /// <summary>
    /// Elimina lógicamente un archivo
    /// </summary>
    /// <param name="id">ID del archivo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("delete-agency-file")]
    [SwaggerOperation(Summary = "Elimina un archivo", Description = "Elimina lógicamente un archivo (lo marca como eliminado).")]
    public async Task<IActionResult> DeleteFile([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.AgencyFilesRepository.DeleteAgencyFile(queryParameters.Id);
        return result ? StatusCode(StatusCodes.Status200OK, new { message = "Archivo eliminado correctamente" }) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
    }

    /// <summary>
    /// Verifica un archivo
    /// </summary>
    /// <param name="id">ID del archivo a verificar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("verify-agency-file")]
    [SwaggerOperation(Summary = "Verifica un archivo", Description = "Marca un archivo como verificado.")]
    public async Task<IActionResult> VerifyFile([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.AgencyFilesRepository.VerifyAgencyFile(queryParameters.Id);
        return result ? StatusCode(StatusCodes.Status200OK, new { message = "Archivo verificado correctamente" }) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
    }
}
