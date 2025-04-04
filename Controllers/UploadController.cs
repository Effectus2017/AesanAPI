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

namespace Api.Controllers
{
    [Route("upload")]
    public class UploadController(IUnitOfWork unitOfWork, IOptions<ConnectionStrings> appConnection, IWebHostEnvironment environment, IOptions<ApplicationSettings> appSettings, ILogger<UploadController> logger) : Controller
    {
        private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        private readonly ConnectionStrings _appConnection = appConnection.Value ?? throw new ArgumentNullException(nameof(appConnection));
        public readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings.Value));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<UploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpPost]
        [Route("uploadFile")]
        [Consumes("multipart/form-data")]
        public async Task<dynamic> UploadFile([FromQuery(Name = "type")] string type, [FromQuery(Name = "fileName")] string fileName, [FromQuery(Name = "folderTo")] string folderTo)
        {
            try
            {
                _logger.LogInformation("Iniciando carga de archivo: {FileName}, tipo: {Type}, carpeta: {FolderTo}", fileName, type, folderTo);

                string folder = string.IsNullOrEmpty(folderTo) ? FilesType.Profile : folderTo;
                string urlPath;
                string fullFileName;

                // Limpiar el nombre del archivo
                fileName = Utilities.RemoveSpecialCharacters(fileName);
                fileName = Utilities.RemoveDiacritics(fileName);

                IFormFileCollection files = Request.Form.Files;
                if (!files.Any())
                {
                    _logger.LogWarning("No se recibieron archivos en la solicitud");
                    return BadRequest(new { error = "No files received" });
                }

                var file = files[0];
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("El archivo está vacío o es nulo");
                    return BadRequest(new { error = "File is empty" });
                }

                string extension = Path.GetExtension(file.FileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                // Generar un nombre único para el archivo
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                fullFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";

                // Verificar el entorno
                bool isAzureEnvironment = !string.IsNullOrEmpty(_appSettings.AzureStorageConnectionString) &&
                                         !_environment.IsDevelopment();

                if (isAzureEnvironment)
                {
                    _logger.LogInformation("Usando Azure Blob Storage para almacenar el archivo");

                    try
                    {
                        // Crear un cliente de blob
                        var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageConnectionString);

                        // Generar un nombre para el contenedor (asegurar que sea minúsculas y válido)
                        string containerName = $"uploads-{folder.ToLower()}";

                        // Obtener una referencia al contenedor y crear si no existe
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        // Obtener una referencia al blob y subir el archivo
                        var blobClient = containerClient.GetBlobClient(fullFileName);

                        _logger.LogInformation("Subiendo archivo a Azure Blob Storage: {ContainerName}/{BlobName}",
                            containerName, fullFileName);

                        using (var stream = file.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                        }

                        // Obtener la URL del blob
                        urlPath = blobClient.Uri.ToString();

                        _logger.LogInformation("Archivo subido exitosamente a Azure Blob Storage: {BlobUrl}", urlPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al subir archivo a Azure Blob Storage, usando almacenamiento local como alternativa");

                        // Si hay error con Azure Blob, usar almacenamiento local como fallback
                        string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", folder);

                        if (!Directory.Exists(uploadsRootFolder))
                        {
                            Directory.CreateDirectory(uploadsRootFolder);
                        }

                        var filePath = Path.Combine(uploadsRootFolder, fullFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream).ConfigureAwait(false);
                        }

                        string url = Utilities.GetUrl(_appSettings);
                        urlPath = $"{url}/uploads/{folder}/{fullFileName}";

                        _logger.LogInformation("Archivo guardado localmente como alternativa: {FilePath}", filePath);
                    }
                }
                else
                {
                    _logger.LogInformation("Usando almacenamiento local para el archivo (entorno de desarrollo)");

                    // Usar almacenamiento local
                    string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", folder);

                    // Crear la carpeta si no existe
                    if (!Directory.Exists(uploadsRootFolder))
                    {
                        Directory.CreateDirectory(uploadsRootFolder);
                    }

                    // Guardar el archivo
                    var filePath = Path.Combine(uploadsRootFolder, fullFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream).ConfigureAwait(false);
                    }

                    // Generar la URL
                    string url = Utilities.GetUrl(_appSettings);
                    urlPath = $"{url}/uploads/{folder}/{fullFileName}";

                    _logger.LogInformation("Archivo guardado localmente: {FilePath}", filePath);
                }

                _logger.LogInformation("URL generada para el archivo: {UrlPath}", urlPath);

                // Normalizar la URL (reemplazar los \ por /)
                urlPath = urlPath.Replace("\\", "/");

                return new { file = fullFileName, urlPath };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<byte[]> GetByteArrayFromImageAsync(IFormFile file)
        {
            using var target = new MemoryStream();
            await file.CopyToAsync(target);
            return target.ToArray();
        }
    }
}
