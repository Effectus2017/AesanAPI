namespace Api.Exceptions;

/// <summary>
/// Excepción de validación de sitio (AESAN-257).
/// Se usa para servicios fuertes obligatorios y tiempo mínimo entre servicios.
/// </summary>
public class SiteValidationException : Exception
{
    public string Code { get; }

    public SiteValidationException(string code, string message)
        : base(message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }

    public SiteValidationException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }
}
