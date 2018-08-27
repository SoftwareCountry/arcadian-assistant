namespace Arcadia.Assistant.Server.WinService
{
    using Akka.Actor;
    using Akka.Monitoring;
    using Akka.Monitoring.ApplicationInsights;

    using Arcadia.Assistant.Configuration.Configuration;

    using NLog;

    public class MonitoredApplication : Application
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private ActorMonitor monitor;

        public MonitoredApplication(AppSettings config)
            : base(config)
        {
        }

        protected override void OnStart(ActorSystem actorSystem)
        {
            base.OnStart(actorSystem);
            var instrumentationKey = this.config.ApplicationInsights.InstrumentationKey;
            Log.Info($"AppInsights key is {instrumentationKey}");
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                this.monitor = ActorMonitoringExtension.Monitors(actorSystem);
                this.monitor.RegisterMonitor(new ActorAppInsightsMonitor(instrumentationKey));
            }
        }

        protected override void OnStop(ActorSystem actorSystem)
        {
            base.OnStop(actorSystem);
            this.monitor?.TerminateMonitors();
            this.monitor = null;
        }
    }
}