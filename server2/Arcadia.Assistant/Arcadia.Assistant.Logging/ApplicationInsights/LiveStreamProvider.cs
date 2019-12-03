namespace Arcadia.Assistant.Logging.ApplicationInsights
{
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

    internal class LiveStreamProvider
    {
        private readonly TelemetryConfiguration configuration;

        public LiveStreamProvider(TelemetryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Enable()
        {
            var module = new DependencyTrackingTelemetryModule();
            module.Initialize(this.configuration);

            QuickPulseTelemetryProcessor processor = null;

            this.configuration.TelemetryProcessorChainBuilder
                .Use(next =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);
                    return processor;
                })
                .Build();

            var quickPulse = new QuickPulseTelemetryModule();
            quickPulse.Initialize(this.configuration);
            quickPulse.RegisterTelemetryProcessor(processor);
        }
    }
}