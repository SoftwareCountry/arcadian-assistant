namespace Arcadia.Assistant.Patterns
{
    using System;

    using Akka.Actor;
    using Akka.Pattern;

    public class PersistenceSupervisorFactory
    {
        public string ChildName { get; set; } = "persist";

        public TimeSpan MinBackoff { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan MaxBackoff { get; set; } = TimeSpan.FromSeconds(30);

        public double RandomFactor { get; set; } = 0.2;

        public Props Get(Props childProps)
        {
            return BackoffSupervisor.Props(Backoff.OnStop(childProps, this.ChildName, this.MinBackoff, this.MaxBackoff, this.RandomFactor));
        }
    }
}