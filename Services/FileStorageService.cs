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

    public async Task<(string fileName, string relativePath)> SaveFileAsync(IFormFile file, string folder, FileType fileType)
    {
        try
        {
            // Limpiar y preparar el nombre del archivo
            string originalFileName = Utilities.RemoveSpecialCharacters(file.FileName);
            originalFileName = Utilities.RemoveDiacritics(originalFileName);

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";

            // Determinar la subcarpeta basada en el tipo de archivo
            string subFolder = GetSubFolder(fileType);
            string fullFolder = Path.Combine(folder, subFolder);

            bool isAzureEnvironment = !string.IsNullOrEmpty(_appSettings.AzureStorageConnectionString) &&
                                    !_environment.IsDevelopment();

            if (isAzureEnvironment)
            {
                try
                {
                    await SaveToAzureStorageAsync(file, fullFolder, fileName);
                    _logger.LogInformation("Archivo guardado exitosamente en Azure Storage: {FileName}", fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar archivo en Azure Storage. Usando almacenamiento local como fallback");
                    await SaveToLocalStorageAsync(file, fullFolder, fileName);
                }
            }
            else
            {
                await SaveToLocalStorageAsync(file, fullFolder, fileName);
            }

            // Construir la ruta relativa del archivo (sin la URL base)
            string relativePath = $"uploads/{folder}/{subFolder}/{fileName}".Replace("\\", "/");

            return (fileName, relativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el archivo");
            throw;
        }
    }

    private static string GetSubFolder(FileType fileType)
    {
        return fileType switch
        {
            FileType.UserAvatar => "avatars",
            FileType.AgencyLogo => "logos",
            FileType.AgencyDocument => "documents",
            _ => throw new ArgumentException("Tipo de archivo no válido", nameof(fileType))
        };
    }

    private async Task SaveToAzureStorageAsync(IFormFile file, string folder, string fileName)
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
    }

    private async Task SaveToLocalStorageAsync(IFormFile file, string folder, string fileName)
    {
        // Crear la ruta física donde se guardará el archivo
        string uploadsRootFolder = Path.Combine(_environment.ContentRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsRootFolder);

        string filePath = Path.Combine(uploadsRootFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        _logger.LogInformation("Archivo guardado localmente: {FilePath}", filePath);
    }

    public async Task<bool> DeleteFileAsync(string fileName, FileType fileType)
    {
        try
        {
            string subFolder = GetSubFolder(fileType);
            bool isAzureEnvironment = !string.IsNullOrEmpty(_appSettings.AzureStorageConnectionString) &&
                                    !_environment.IsDevelopment();

            if (isAzureEnvironment)
            {
                try
                {
                    var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageConnectionString);
                    string containerName = subFolder.ToLower().Replace('/', '-');
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    return await blobClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar archivo de Azure Storage. Intentando eliminar localmente como fallback");
                    return await DeleteFromLocalStorageAsync(subFolder, fileName);
                }
            }

            return await DeleteFromLocalStorageAsync(subFolder, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el archivo {FileName}", fileName);
            return false;
        }
    }

    private async Task<bool> DeleteFromLocalStorageAsync(string subFolder, string fileName)
    {
        string filePath = Path.Combine(_environment.ContentRootPath, "uploads", subFolder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }
}