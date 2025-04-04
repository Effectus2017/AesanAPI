namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para subir archivos a una agencia
/// </summary>
public class AgencyFileRequest
{
    /// <summary>
    /// Identificador de la agencia a la que pertenece el archivo
    /// </summary>
    public int AgencyId { get; set; }

    /// <summary>
    /// URL del archivo que se ha subido previamente
    /// </summary>
    public string FileUrl { get; set; } = "";

    /// <summary>
    /// Nombre original del archivo
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// Nombre del archivo en el sistema
    /// </summary>
    public string StoredFileName { get; set; } = "";

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
}