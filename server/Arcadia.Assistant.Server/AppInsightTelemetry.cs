namespace Arcadia.Assistant.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

    public class AppInsightTelemetry : IDisposable
    {
        private readonly TelemetryConfiguration configuration;

        private readonly List<ITelemetryModule> initializedModules = new List<ITelemetryModule>();

        public AppInsightTelemetry(TelemetryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Run()
        {
            foreach (var telemetryInitializer in this.GetInitializers())
            {
                this.configuration.TelemetryInitializers.Add(telemetryInitializer);
            }

            foreach (var telemetryModule in this.GetModules())
            {
                telemetryModule.Initialize(this.configuration);
                this.initializedModules.Add(telemetryModule);
            }
        }

        public TelemetryClient CreateTelemetryClient()
        {
            return new TelemetryClient(this.configuration);
        }

        protected virtual IList<ITelemetryInitializer> GetInitializers()
        {
            return new List<ITelemetryInitializer>()
                {
                    new HttpDependenciesParsingTelemetryInitializer(),
                    new ServerTelemetryInitializer(),
                    new OperationCorrelationTelemetryInitializer()
                };
        }

        protected virtual IList<ITelemetryModule> GetModules()
        {
            return new List<ITelemetryModule>()
                {
                    new PerformanceCollectorModule(),
                    this.GetDependencyTrackingModule()
                };
        }

        protected virtual ITelemetryModule GetDependencyTrackingModule()
        {
            var depTrackingModule = new DependencyTrackingTelemetryModule();
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.chinacloudapi.cn");
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.cloudapi.de");
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.usgovcloudapi.net");
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("localhost");
            depTrackingModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("127.0.0.1");

            depTrackingModule.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
            depTrackingModule.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

            return depTrackingModule;
        }

        private void InitializeQuickPulse()
        {
            var quickPulseModule = new QuickPulseTelemetryModule();
            quickPulseModule.Initialize(this.configuration);

            QuickPulseTelemetryProcessor processor = null;
            this.configuration.TelemetryProcessorChainBuilder.Use(
                    next =>
                        {
                            processor = new QuickPulseTelemetryProcessor(next);
                            return processor;
                        })
                .Build();

            quickPulseModule.RegisterTelemetryProcessor(processor);

            this.initializedModules.Add(quickPulseModule);
        }

        public void Dispose()
        {
            this.configuration.Dispose();
            foreach (var initializedModule in this.initializedModules)
            {
                if (initializedModule is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            this.initializedModules.Clear();
        }
    }
}