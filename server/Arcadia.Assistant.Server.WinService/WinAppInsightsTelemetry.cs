namespace Arcadia.Assistant.Server.WinService
{
    using System.Collections.Generic;

    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
    using Microsoft.ApplicationInsights.WindowsServer;

    public class WinAppInsightsTelemetry : AppInsightTelemetry
    {
        public WinAppInsightsTelemetry(TelemetryConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IList<ITelemetryInitializer> GetInitializers()
        {
            var initializers = base.GetInitializers();
            initializers.Add(new DeviceTelemetryInitializer());
            return initializers;
        }

        protected override IList<ITelemetryModule> GetModules()
        {
            var modules = base.GetModules();
            modules.Add(new FirstChanceExceptionStatisticsTelemetryModule());
            modules.Add(new UnhandledExceptionTelemetryModule());
            modules.Add(new UnobservedExceptionTelemetryModule());
            modules.Add(new AzureInstanceMetadataTelemetryModule());
            return modules;
        }
    }
}