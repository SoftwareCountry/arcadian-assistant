namespace Arcadia.Assistant.Server.WinService
{
    using Akka.Actor;
    using Akka.Monitoring;
    using Akka.Monitoring.ApplicationInsights;

    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.Extensions.Configuration;

    public class MonitoredApplication : Application
    {
        private readonly AppInsightTelemetry telemetry;

        /// <summary>
        /// ActorMonitor is netFramework only, hence the separate class
        /// </summary>
        private ActorMonitor monitor;

        public MonitoredApplication(IConfigurationRoot config)
            : base(config)
        {
            var settings = config.Get<AppSettings>();
            this.telemetry = new WinAppInsightsTelemetryFactory(settings).Create();
        }

        protected override void OnStart(ActorSystem actorSystem)
        {
            base.OnStart(actorSystem);
            this.telemetry.Run();
            this.monitor = ActorMonitoringExtension.Monitors(actorSystem);
            this.monitor.RegisterMonitor(new ActorAppInsightsMonitor(this.telemetry.CreateTelemetryClient()));
        }

        protected override void OnStop(ActorSystem actorSystem)
        {
            base.OnStop(actorSystem);
            this.telemetry.Dispose();
            this.monitor?.TerminateMonitors();
            this.monitor = null;
        }
    }
}