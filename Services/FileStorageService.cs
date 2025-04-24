using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Api.Models.Enums;
using Api.Models;
using Api.Interfaces;

namespace Api.Services;



public class FileStorageService(IWebHostEnvironment environment, IOptions<ApplicationSettings> appSettings, ILogger<FileStorageService> logger) : IFileStorageService
{
    private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly ILogger<FileStorageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<(string fileName, string fileUrl)> SaveFileAsync(IFormFile file, string folder, FileType fileType)
    {
        try
        {
            // Limpiar y preparar el nombre del archivo
            string fileName = Utilities.RemoveSpecialCharacters(file.FileName);
            fileName = Utilities.RemoveDiacritics(fileName);

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string storedFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";

            // Determinar la subcarpeta basada en el tipo de archivo
            string subFolder = GetSubFolder(fileType);
            string fullFolder = Path.Combine(folder, subFolder);

            bool isAzureEnvironment = !string.IsNullOrEmpty(_appSettings.AzureStorageConnectionString) &&
                                    !_environment.IsDevelopment();

            string fileUrl;

            if (isAzureEnvironment)
            {
                try
                {
                    fileUrl = await SaveToAzureStorageAsync(file, fullFolder, storedFileName);
                    _logger.LogInformation("Archivo guardado exitosamente en Azure Storage: {FileName}", storedFileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar archivo en Azure Storage. Usando almacenamiento local como fallback");
                    fileUrl = await SaveToLocalStorageAsync(file, fullFolder, storedFileName);
                }
            }
            else
            {
                fileUrl = await SaveToLocalStorageAsync(file, fullFolder, storedFileName);
            }

            return (storedFileName, fileUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el archivo");
            throw;
        }
    }

    private string GetSubFolder(FileType fileType)
    {
        return fileType switch
        {
            FileType.UserAvatar => "avatars",
            FileType.AgencyLogo => "logos",
            FileType.AgencyDocument => "documents",
            _ => throw new ArgumentException("Tipo de archivo no válido", nameof(fileType))
        };
    }

    private async Task<string> SaveToAzureStorageAsync(IFormFile file, string folder, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageConnectionString);
        string containerName = folder.ToLower().Replace('/', '-');

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
        }

        return blobClient.Uri.ToString();
    }

    private async Task<string> SaveToLocalStorageAsync(IFormFile file, string folder, string fileName)
    {
        string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsRootFolder);

        string filePath = Path.Combine(uploadsRootFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        string url = Utilities.GetUrl(_appSettings);
        return $"{url}/uploads/{folder}/{fileName}";
    }
}