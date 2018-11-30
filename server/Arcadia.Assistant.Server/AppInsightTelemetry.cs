﻿namespace Arcadia.Assistant.Server
{
    using System;
    using System.Collections.Generic;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

    using NLog;

    public class AppInsightTelemetry : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly TelemetryConfiguration configuration;

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
            }

            this.InitializeQuickPulse();
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

        protected virtual IEnumerable<ITelemetryModule> GetModules()
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
        }

        public void Dispose()
        {
            this.configuration.Dispose();
        }
    }
}