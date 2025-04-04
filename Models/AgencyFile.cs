using System;

namespace Api.Models;

/// <summary>
/// Modelo para representar los archivos asociados a una agencia
/// </summary>
public class AgencyFile
{
    /// <summary>
    /// Identificador único del archivo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la agencia a la que pertenece el archivo
    /// </summary>
    public int AgencyId { get; set; }

    /// <summary>
    /// Nombre original del archivo
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// Nombre del archivo en el sistema
    /// </summary>
    public string StoredFileName { get; set; } = "";

    /// <summary>
    /// Ruta URL donde se puede acceder al archivo
    /// </summary>
    public string FileUrl { get; set; } = "";

    /// <summary>
    /// Tipo de contenido del archivo (MIME type)
    /// </summary>
    public string ContentType { get; set; } = "";

    /// <summary>
    /// Tamaño del archivo en bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Descripción del archivo
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Tipo de documento (puede ser un enum o un ID a una tabla de tipos)
    /// </summary>
    public string DocumentType { get; set; } = "";

    /// <summary>
    /// Fecha de subida del archivo
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Usuario que subió el archivo
    /// </summary>
    public string UploadedBy { get; set; } = "";

    /// <summary>
    /// Indica si el archivo está activo o ha sido eliminado
    /// </summary>
    public bool IsActive { get; set; } = true;
}