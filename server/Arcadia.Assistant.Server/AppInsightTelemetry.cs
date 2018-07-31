namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.Configuration;

    public class AppInsightTelemetry
    {
        public void Setup(AppSettings settings)
        {
            TelemetryConfiguration.Active.InstrumentationKey = settings.ApplicationInsights?.InstrumentationKey;
        }
    }
}