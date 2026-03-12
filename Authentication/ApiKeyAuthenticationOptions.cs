using Microsoft.AspNetCore.Authentication;

namespace Api.Authentication;

/// <summary>
/// Opciones para el esquema de autenticación por X-Api-Key (uso interno / Web Job en /notifications).
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";

    /// <summary>
    /// Clave esperada; debe coincidir con ApplicationSettings.InternalApiKey.
    /// </summary>
    public string? ExpectedApiKey { get; set; }
}
