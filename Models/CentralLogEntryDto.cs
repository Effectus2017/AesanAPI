namespace Api.Models;

/// <summary>
/// DTO unificado para entradas del centro de logs (todas las categorías).
/// Usado por GET /api/logs para respuesta común.
/// </summary>
public class CentralLogEntryDto
{
    public string Category { get; set; } = "";
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Summary { get; set; } = "";
    public string? Status { get; set; }
    public string? Payload { get; set; }
    public string? UserId { get; set; }
    public string? Level { get; set; }

    /// <summary>
    /// Rellenado por los SPs paginados (COUNT(*) OVER()); solo la primera fila tiene valor.
    /// No se expone en la API; se usa para devolver totalCount en la respuesta paginada.
    /// </summary>
    public int? TotalCount { get; set; }
}
