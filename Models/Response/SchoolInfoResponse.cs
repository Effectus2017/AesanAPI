namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para información básica de una escuela
/// </summary>
public class SchoolInfoResponse
{
    /// <summary>
    /// ID de la escuela
    /// </summary>
    public int SchoolId { get; set; }

    /// <summary>
    /// Nombre de la escuela
    /// </summary>
    public string SchoolName { get; set; } = string.Empty;
}
