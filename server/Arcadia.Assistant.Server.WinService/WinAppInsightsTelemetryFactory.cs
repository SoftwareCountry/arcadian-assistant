namespace Arcadia.Assistant.Server.WinService
{
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.ApplicationInsights.Extensibility;

    public class WinAppInsightsTelemetryFactory : AppInsightTelemetryFactory
    {
        public WinAppInsightsTelemetryFactory(AppSettings settings)
            : base(settings)
        {
        }

        protected override AppInsightTelemetry Create(TelemetryConfiguration configuration)
        {
            return new WinAppInsightsTelemetry(configuration);
        }
    }
}