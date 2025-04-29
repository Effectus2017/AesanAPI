using Microsoft.AspNetCore.Http;
using Api.Models.Enums;

namespace Api.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Guarda un archivo y retorna el nombre del archivo guardado y su ruta relativa
    /// </summary>
    /// <param name="file">Archivo a guardar</param>
    /// <param name="folder">Carpeta donde se guardará</param>
    /// <param name="fileType">Tipo de archivo</param>
    /// <returns>Tupla con el nombre del archivo guardado y su ruta relativa</returns>
    Task<(string fileName, string relativePath)> SaveFileAsync(IFormFile file, string folder, FileType fileType);

    Task<bool> DeleteFileAsync(string fileName, FileType fileType);
}