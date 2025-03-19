using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Channel;

namespace Api.Services;

public interface ILoggingService
{
    void LogError(Exception ex, string message = null, IDictionary<string, string> properties = null);
    void LogWarning(string message, IDictionary<string, string> properties = null);
    void LogInformation(string message, IDictionary<string, string> properties = null);
}

public class LoggingService : ILoggingService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly IWebHostEnvironment _environment;

    public LoggingService(TelemetryClient telemetryClient, IWebHostEnvironment environment)
    {
        _telemetryClient = telemetryClient;
        _environment = environment;
    }

    public void LogError(Exception ex, string message = null, IDictionary<string, string> properties = null)
    {
        var telemetry = new ExceptionTelemetry(ex);
        EnrichTelemetry(telemetry, properties);

        if (!string.IsNullOrEmpty(message))
        {
            telemetry.Properties["CustomMessage"] = message;
        }

        _telemetryClient.TrackException(telemetry);

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