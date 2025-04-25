namespace Api.Models;

/// <summary>
/// DTO para transferir datos de archivos de agencia
/// </summary>
public class DTOAgencyFile
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
    /// Nombre de la agencia (para mostrar en la UI)
    /// </summary>
    public string AgencyName { get; set; } = "";

    /// <summary>
    /// Nombre original del archivo
    /// </summary>
    public string FileName { get; set; } = "";

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
    /// Tipo de documento
    /// </summary>
    public string DocumentType { get; set; } = "";

    /// <summary>
    /// Fecha de subida del archivo
    /// </summary>
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// Nombre del usuario que subió el archivo (para mostrar en la UI)
    /// </summary>
    public string UploadedByName { get; set; } = "";
}