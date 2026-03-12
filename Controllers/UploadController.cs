using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja la subida de archivos.
/// </summary>
[ApiController]
[Route("upload")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class UploadController(IUnitOfWork unitOfWork, IFileStorageService fileStorageService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IFileStorageService _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));

    [HttpPost]
    [Route("upload-agency-file")]
    [Consumes("multipart/form-data")]
    public async Task<dynamic> UploadAgencyFile(
        [FromQuery(Name = "agencyId")] int agencyId,
        [FromQuery(Name = "userId")] string userId,
        [FromQuery(Name = "description")] string description,
        [FromQuery(Name = "documentType")] string documentType
        )
    {
        var files = Request.Form.Files;
        if (!files.Any())
        {
            return BadRequest(new { error = "No se recibieron archivos" });
        }

        var file = files[0];
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "El archivo está vacío" });
        }

        // Guardar el archivo usando el servicio
        var (storedFileName, relativePath) = await _fileStorageService.SaveFile(
            file,
            $"agency_{agencyId}",
            FileType.AgencyDocument
        );

        // Crear registro en la base de datos usando el repositorio
        var agencyFile = new AgencyFileRequest
        {
            AgencyId = agencyId,
            FileName = file.FileName,
            StoredFileName = storedFileName,
            FileUrl = relativePath,
            ContentType = file.ContentType,
            FileSize = file.Length,
            Description = description,
            DocumentType = documentType,
            UploadedBy = User.Identity?.Name ?? userId
        };

        try
        {
            var newFileId = await _unitOfWork.AgencyFilesRepository.AddAgencyFile(agencyFile);
            return new { file = storedFileName, url = relativePath, id = newFileId };
        }
        catch (InvalidOperationException ex)
        {
            // Si la agencia no existe, eliminamos el archivo que acabamos de guardar
            await _fileStorageService.DeleteFile(storedFileName, FileType.AgencyDocument);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Route("upload-agency-logo")]
    [Consumes("multipart/form-data")]
    public async Task<dynamic> UploadAgencyLogo([FromQuery(Name = "agencyId")] int agencyId)
    {
        var files = Request.Form.Files;
        if (!files.Any())
        {
            return BadRequest(new { error = "No se recibieron archivos" });
        }

        var file = files[0];
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "El archivo está vacío" });
        }

        // Validar que sea una imagen
        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest(new { error = "El archivo debe ser una imagen" });
        }

        // Guardar el archivo usando el servicio
        var (storedFileName, fileUrl) = await _fileStorageService.SaveFile(
            file,
            $"agency_{agencyId}",
            FileType.AgencyLogo
        );

        // Actualizar el logo en la agencia
        await _unitOfWork.AgencyRepository.UpdateAgencyLogo(agencyId, fileUrl);

        return new { file = storedFileName, url = fileUrl };
    }

    [HttpPost]
    [Route("upload-user-avatar")]
    [Consumes("multipart/form-data")]
    public async Task<dynamic> UploadUserAvatar([FromQuery(Name = "userId")] string userId)
    {
        var files = Request.Form.Files;
        if (!files.Any())
        {
            return BadRequest(new { error = "No se recibieron archivos" });
        }

        var file = files[0];
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "El archivo está vacío" });
        }

        // Validar que sea una imagen
        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest(new { error = "El archivo debe ser una imagen" });
        }

        // Guardar el archivo usando el servicio
        var (storedFileName, fileUrl) = await _fileStorageService.SaveFile(
            file,
            $"user_{userId}",
            FileType.UserAvatar
        );

        // Actualizar el avatar en el usuario
        await _unitOfWork.UserRepository.UpdateUserAvatar(userId, fileUrl);

        return new { file = storedFileName, url = fileUrl };
    }
}
