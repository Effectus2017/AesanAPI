using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("upload")]
public class UploadController(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ILogger<UploadController> logger) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IFileStorageService _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    private readonly ILogger<UploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
        try
        {
            _logger.LogInformation($"Iniciando carga de archivo para la agencia {agencyId}");

            var files = Request.Form.Files;
            if (!files.Any())
            {
                _logger.LogWarning("No se recibieron archivos en la solicitud");
                return BadRequest(new { error = "No se recibieron archivos" });
            }

            var file = files[0];
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("El archivo está vacío o es nulo");
                return BadRequest(new { error = "El archivo está vacío" });
            }

            // Guardar el archivo usando el servicio
            var (storedFileName, relativePath) = await _fileStorageService.SaveFileAsync(
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
                UploadedBy = User.Identity.Name ?? userId
            };

            var newFileId = await _unitOfWork.AgencyFilesRepository.AddAgencyFile(agencyFile);

            return new { file = storedFileName, url = relativePath, id = newFileId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir archivo");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Route("upload-agency-logo")]
    [Consumes("multipart/form-data")]
    public async Task<dynamic> UploadAgencyLogo([FromQuery(Name = "agencyId")] int agencyId)
    {
        try
        {
            _logger.LogInformation($"Iniciando carga de logo para la agencia {agencyId}");

            var files = Request.Form.Files;
            if (!files.Any())
            {
                _logger.LogWarning("No se recibieron archivos en la solicitud");
                return BadRequest(new { error = "No se recibieron archivos" });
            }

            var file = files[0];
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("El archivo está vacío o es nulo");
                return BadRequest(new { error = "El archivo está vacío" });
            }

            // Validar que sea una imagen
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { error = "El archivo debe ser una imagen" });
            }

            // Guardar el archivo usando el servicio
            var (storedFileName, fileUrl) = await _fileStorageService.SaveFileAsync(
                file,
                $"agency_{agencyId}",
                FileType.AgencyLogo
            );

            // Actualizar el logo en la agencia
            await _unitOfWork.AgencyRepository.UpdateAgencyLogo(agencyId, fileUrl);

            return new { file = storedFileName, url = fileUrl };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir logo");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Route("upload-user-avatar")]
    [Consumes("multipart/form-data")]
    public async Task<dynamic> UploadUserAvatar([FromQuery(Name = "userId")] string userId)
    {
        try
        {
            _logger.LogInformation($"Iniciando carga de avatar para el usuario {userId}");

            var files = Request.Form.Files;
            if (!files.Any())
            {
                _logger.LogWarning("No se recibieron archivos en la solicitud");
                return BadRequest(new { error = "No se recibieron archivos" });
            }

            var file = files[0];
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("El archivo está vacío o es nulo");
                return BadRequest(new { error = "El archivo está vacío" });
            }

            // Validar que sea una imagen
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { error = "El archivo debe ser una imagen" });
            }

            // Guardar el archivo usando el servicio
            var (storedFileName, fileUrl) = await _fileStorageService.SaveFileAsync(
                file,
                $"user_{userId}",
                FileType.UserAvatar
            );

            // Actualizar el avatar en el usuario
            await _unitOfWork.UserRepository.UpdateUserAvatar(userId, fileUrl);

            return new { file = storedFileName, url = fileUrl };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir avatar");
            return BadRequest(new { error = ex.Message });
        }
    }
}

