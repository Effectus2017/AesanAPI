using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.AspNetCore.Http;
using Api.Interfaces;

namespace Api.Services;

public interface ILoggingService
{
    Task LogError(Exception ex, string? message = null, IDictionary<string, string>? properties = null);
    void LogWarning(string message, IDictionary<string, string>? properties = null);
    void LogInformation(string message, IDictionary<string, string>? properties = null);
}

public class LoggingService(
    TelemetryClient telemetryClient,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    ICentralLogService centralLogService) : ILoggingService
{
    private readonly TelemetryClient _telemetryClient = telemetryClient;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ICentralLogService _centralLogService = centralLogService ?? throw new ArgumentNullException(nameof(centralLogService));

    public async Task LogError(Exception ex, string? message = null, IDictionary<string, string>? properties = null)
    {
        var telemetry = new ExceptionTelemetry(ex);
        EnrichTelemetry(telemetry, properties);

        if (!string.IsNullOrEmpty(message))
        {
            telemetry.Properties["CustomMessage"] = message;
        }

        _telemetryClient.TrackException(telemetry);

        // Log to central system (LogApplication) - sustituye a ELMAH
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var payload = new Dictionary<string, object?>
        {
            ["ExceptionType"] = ex.GetType().FullName,
            ["StackTrace"] = ex.StackTrace,
            ["CustomMessage"] = message
        };
        if (properties != null)
        {
            foreach (var p in properties)
                payload[p.Key] = p.Value;
        }
        var payloadJson = JsonSerializer.Serialize(payload);
        try
        {
            await _centralLogService.LogApplicationAsync("Error", message ?? ex.Message, payloadJson, userId, null);
        }
        catch
        {
            // No fallar si el log central falla
        }

        // Log to console in development
        if (_environment.IsDevelopment())
        {
            Console.Error.WriteLine($"Error: {message ?? ex.Message}");
            Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

    public void LogWarning(string message, IDictionary<string, string>? properties = null)
    {
        var telemetry = new TraceTelemetry(message, SeverityLevel.Warning);
        EnrichTelemetry(telemetry, properties);
        _telemetryClient.TrackTrace(telemetry);

        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"Warning: {message}");
        }
    }

    public void LogInformation(string message, IDictionary<string, string>? properties = null)
    {
        var telemetry = new TraceTelemetry(message, SeverityLevel.Information);
        EnrichTelemetry(telemetry, properties);
        _telemetryClient.TrackTrace(telemetry);

        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"Info: {message}");
        }
    }

    private void EnrichTelemetry(ITelemetry telemetry, IDictionary<string, string>? properties)
    {
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                telemetry.Context.GlobalProperties[prop.Key] = prop.Value;
            }
        }
    }
}