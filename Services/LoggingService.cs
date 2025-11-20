using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Channel;
using ElmahCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services;

public interface ILoggingService
{
    Task LogError(Exception ex, string message = null, IDictionary<string, string> properties = null);
    void LogWarning(string message, IDictionary<string, string> properties = null);
    void LogInformation(string message, IDictionary<string, string> properties = null);
}

public class LoggingService(
    TelemetryClient telemetryClient,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor) : ILoggingService
{
    private readonly TelemetryClient _telemetryClient = telemetryClient;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task LogError(Exception ex, string message = null, IDictionary<string, string> properties = null)
    {
        var telemetry = new ExceptionTelemetry(ex);
        EnrichTelemetry(telemetry, properties);

        if (!string.IsNullOrEmpty(message))
        {
            telemetry.Properties["CustomMessage"] = message;
        }

        _telemetryClient.TrackException(telemetry);

        // Log to ELMAH
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var errorLog = context.RequestServices.GetService<ErrorLog>();
            if (errorLog != null)
            {
                await errorLog.LogAsync(new Error(ex, context));
            }
        }

        // Log to console in development
        if (_environment.IsDevelopment())
        {
            Console.Error.WriteLine($"Error: {message ?? ex.Message}");
            Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

    public void LogWarning(string message, IDictionary<string, string> properties = null)
    {
        var telemetry = new TraceTelemetry(message, SeverityLevel.Warning);
        EnrichTelemetry(telemetry, properties);
        _telemetryClient.TrackTrace(telemetry);

        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"Warning: {message}");
        }
    }

    public void LogInformation(string message, IDictionary<string, string> properties = null)
    {
        var telemetry = new TraceTelemetry(message, SeverityLevel.Information);
        EnrichTelemetry(telemetry, properties);
        _telemetryClient.TrackTrace(telemetry);

        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"Info: {message}");
        }
    }

    private void EnrichTelemetry(ITelemetry telemetry, IDictionary<string, string> properties)
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