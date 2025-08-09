namespace Api.Models;

/// <summary>
/// Clase para respuestas de error consistentes en toda la API
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Mensaje principal del error
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Detalles específicos del error
    /// </summary>
    public string Details { get; set; } = "";

    /// <summary>
    /// Stack trace del error (solo en desarrollo)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Excepción interna si existe
    /// </summary>
    public string? InnerException { get; set; }

    /// <summary>
    /// Código de error específico
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp del error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public ErrorResponse() { }

    /// <summary>
    /// Constructor con mensaje
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    public ErrorResponse(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Constructor con mensaje y detalles
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles del error</param>
    public ErrorResponse(string message, string details)
    {
        Message = message;
        Details = details;
    }

    /// <summary>
    /// Constructor completo
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles del error</param>
    /// <param name="stackTrace">Stack trace</param>
    /// <param name="innerException">Excepción interna</param>
    /// <param name="errorCode">Código de error</param>
    public ErrorResponse(string message, string details, string? stackTrace = null, string? innerException = null, string? errorCode = null)
    {
        Message = message;
        Details = details;
        StackTrace = stackTrace;
        InnerException = innerException;
        ErrorCode = errorCode;
    }
}