namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.ApplicationInsights.Extensibility;

    using NLog;

    public class AppInsightTelemetryFactory
    {
        private readonly AppSettings settings;

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public AppInsightTelemetryFactory(AppSettings settings)
        {
            this.settings = settings;
        }

        public AppInsightTelemetry Create()
        {
            var configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = this.settings.ApplicationInsights?.InstrumentationKey;
            Log.Info($"TelemetryConfiguration is set to {configuration.InstrumentationKey}");

            var telemetry = this.Create(configuration);
            return telemetry;
        }

        protected virtual AppInsightTelemetry Create(TelemetryConfiguration configuration)
        {
            return new AppInsightTelemetry(configuration);
        }
    }
}