namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.ApplicationInsights.Extensibility;

    using NLog;

    public class AppInsightTelemetry
    {
        private readonly AppSettings settings;

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public AppInsightTelemetry(AppSettings settings)
        {
            this.settings = settings;
        }

        public void Setup()
        {
            TelemetryConfiguration.Active.InstrumentationKey = this.settings.ApplicationInsights?.InstrumentationKey;
            Log.Info($"TelemetryConfiguration is set to {TelemetryConfiguration.Active.InstrumentationKey}");
        }

        public void Dispose()
        {
        }
    }
}