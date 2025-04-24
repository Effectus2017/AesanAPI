using Api.Models.Enums;

namespace Api.Interfaces;

public interface IFileStorageService
{
    Task<(string fileName, string fileUrl)> SaveFileAsync(IFormFile file, string folder, FileType fileType);
}