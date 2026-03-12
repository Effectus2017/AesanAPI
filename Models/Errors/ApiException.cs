namespace Api.Models.Errors;

using System.Text.Json.Serialization;

/// <summary>
/// Modelo único y excepción base para todos los errores de la API.
/// Se usa tanto para lanzar excepciones como para la estructura de respuesta JSON.
/// </summary>
public class ApiException(ErrorCode code, string message, int statusCode = 400, Exception? innerException = null) : Exception(message, innerException)
{
    /// <summary>
    /// Constructor para errores inesperados que incluyen la excepción interna (p. ej. en catch).
    /// Usa statusCode 500 por defecto.
    /// </summary>
    public ApiException(ErrorCode code, string message, Exception innerException, int statusCode = 500)
        : this(code, message, statusCode, innerException)
    {
    }

    [JsonPropertyName("code")]
    public ErrorCode Code { get; set; } = code;

    [JsonPropertyName("message")]
    public override string Message { get; } = message;

    [JsonIgnore]
    public int StatusCode { get; set; } = statusCode;
}
