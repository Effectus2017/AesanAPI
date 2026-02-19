namespace Api.Models.Request;

/// <summary>
/// Request para insertar un registro en LogApplication (errores de aplicación).
/// Esquema unificado del sistema central de logging.
/// </summary>
public class LogApplicationRequest
{
    public string Category { get; set; } = "Application";
    public string? Level { get; set; }
    public string Message { get; set; } = "";
    public string? Payload { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }
}
