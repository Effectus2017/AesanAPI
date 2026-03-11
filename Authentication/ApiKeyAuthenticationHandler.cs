using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Api.Authentication;

/// <summary>
/// Valida el header X-Api-Key y establece el principal para rutas que usan [Authorize(AuthenticationSchemes = "ApiKey")].
/// Uso interno / Web Job (ej. POST /notifications/job-log, /notifications/send-welcome-agency).
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var expectedKey = Options.ExpectedApiKey?.Trim();
        if (string.IsNullOrEmpty(expectedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Internal API Key no configurada."));
        }

        var providedKey = Request.Headers[ApiKeyAuthenticationOptions.HeaderName].FirstOrDefault();
        if (string.IsNullOrEmpty(providedKey) || !string.Equals(expectedKey, providedKey.Trim(), StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("X-Api-Key inválida o faltante."));
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "InternalApiKey") },
            Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
