namespace Arcadia.Assistant.Server.WinService
{
    using Akka.Actor;
    using Akka.Monitoring;
    using Akka.Monitoring.ApplicationInsights;

    using Microsoft.Extensions.Configuration;

    public class MonitoredApplication : Application
    {
        public MonitoredApplication(IConfigurationRoot config)
            : base(config)
        {
        }

        protected override void OnStart(ActorSystem actorSystem)
        {
            base.OnStart(actorSystem);
            var instrumentationKey = this.config.GetValue<string>("ApplicationInsights:InstrumentationKey");
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                ActorMonitoringExtension.RegisterMonitor(actorSystem, new ActorAppInsightsMonitor(instrumentationKey));
            }
        }

        protected override void OnStop(ActorSystem actorSystem)
        {
            base.OnStop(actorSystem);
            ActorMonitoringExtension.TerminateMonitors(actorSystem);
        }
    }
}