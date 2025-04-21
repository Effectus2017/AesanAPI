using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Api.Models.Request;

namespace Api.Controllers
{
    [Route("upload")]
    public class UploadController(
        IUnitOfWork unitOfWork,
        IOptions<ConnectionStrings> appConnection,
        IWebHostEnvironment environment,
        IOptions<ApplicationSettings> appSettings,
        ILogger<UploadController> logger) : Controller
    {
        private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        private readonly ConnectionStrings _appConnection = appConnection.Value ?? throw new ArgumentNullException(nameof(appConnection));
        private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<UploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpPost]
        [Route("agency/{agencyId}/file")]
        public async Task<IActionResult> UploadAgencyFile(int agencyId, [FromQuery] string description = null, [FromQuery] string documentType = null)
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

                // Limpiar el nombre del archivo
                string fileName = Utilities.RemoveSpecialCharacters(file.FileName);
                fileName = Utilities.RemoveDiacritics(fileName);

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string storedFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";

                // Definir la carpeta específica para la agencia
                string agencyFolder = $"agency_{agencyId}";
                string fileUrl;

                // Verificar el entorno
                bool isAzureEnvironment = !string.IsNullOrEmpty(_appSettings.AzureStorageConnectionString) &&
                                         !_environment.IsDevelopment();

                if (isAzureEnvironment)
                {
                    try
                    {
                        var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageConnectionString);
                        string containerName = $"agency-files-{agencyId}".ToLower();

                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        var blobClient = containerClient.GetBlobClient(storedFileName);
                        using (var stream = file.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                        }

                        fileUrl = blobClient.Uri.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al subir archivo a Azure Blob Storage");

                        // Fallback a almacenamiento local
                        string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", agencyFolder);
                        Directory.CreateDirectory(uploadsRootFolder);

                        string filePath = Path.Combine(uploadsRootFolder, storedFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        string url = Utilities.GetUrl(_appSettings);
                        fileUrl = $"{url}/uploads/{agencyFolder}/{storedFileName}";
                    }
                }
                else
                {
                    string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", agencyFolder);
                    Directory.CreateDirectory(uploadsRootFolder);

                    string filePath = Path.Combine(uploadsRootFolder, storedFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    string url = Utilities.GetUrl(_appSettings);
                    fileUrl = $"{url}/uploads/{agencyFolder}/{storedFileName}";
                }

                // Crear registro en la base de datos usando el repositorio
                var userId = User.Identity.Name; // Obtener el usuario actual
                var newFileId = await _unitOfWork.AgencyFilesRepository.AddAgencyFile(new AgencyFileRequest
                {
                    AgencyId = agencyId,
                    FileName = fileName,
                    StoredFileName = storedFileName,
                    FileUrl = fileUrl,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    Description = description,
                    DocumentType = documentType,
                    UploadedBy = userId
                }, userId);

                return Ok(new
                {
                    id = newFileId,
                    file = storedFileName,
                    url = fileUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
