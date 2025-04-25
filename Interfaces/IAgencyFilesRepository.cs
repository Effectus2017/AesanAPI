using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

/// <summary>
/// Interfaz que define las operaciones para el repositorio de archivos de agencia
/// </summary>
public interface IAgencyFilesRepository
{
    /// <summary>
    /// Obtiene todos los archivos asociados a una agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="documentType">Tipo de documento (opcional)</param>
    /// <param name="name">Nombre del archivo (opcional)</param>
    /// <returns>Lista paginada de archivos de la agencia</returns>
    Task<dynamic> GetAgencyFiles(int agencyId, int take, int skip, string name = null, string documentType = null);

    /// <summary>
    /// Obtiene un archivo específico por su ID
    /// </summary>
    /// <param name="id">ID del archivo</param>
    /// <returns>El archivo solicitado o null si no existe</returns>
    Task<DTOAgencyFile> GetAgencyFileById(int id);

    /// <summary>
    /// Agrega un nuevo archivo a una agencia
    /// </summary>
    /// <param name="request">Datos del archivo a agregar</param>
    /// <param name="uploadedBy">ID del usuario que sube el archivo</param>
    /// <returns>ID del archivo agregado</returns>
    Task<int> AddAgencyFile(AgencyFileRequest request);

    /// <summary>
    /// Actualiza la información de un archivo
    /// </summary>
    /// <param name="id">ID del archivo</param>
    /// <param name="description">Nueva descripción (opcional)</param>
    /// <param name="documentType">Nuevo tipo de documento (opcional)</param>
    /// <returns>True si la actualización fue exitosa</returns>
    Task<bool> UpdateAgencyFile(int id, string description = null, string documentType = null);

    /// <summary>
    /// Elimina lógicamente un archivo (lo marca como inactivo)
    /// </summary>
    /// <param name="id">ID del archivo a eliminar</param>
    /// <returns>True si la eliminación fue exitosa</returns>
    Task<bool> DeleteAgencyFile(int id);

    /// <summary>
    /// Verifica un archivo de agencia
    /// </summary>
    /// <param name="id">ID del archivo a verificar</param>
    /// <returns>True si la verificación fue exitosa</returns>
    Task<bool> VerifyAgencyFile(int id);
}