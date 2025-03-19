using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Api.Telemetry;

public class EnvironmentTelemetryInitializer : ITelemetryInitializer
{
    private readonly IWebHostEnvironment _environment;

    public EnvironmentTelemetryInitializer(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties["Environment"] = _environment.EnvironmentName;
        telemetry.Context.GlobalProperties["ApplicationName"] = "AESAN.API";
    }
}