namespace Arcadia.Assistant.Health
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Net;
    using System.Threading.Tasks;

    using Abstractions;
    using Akka.Actor;
    using CSP;

    public class HealthActor : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HealthCheckMessage _:
                    var healthResult = new Dictionary<string, bool>
                    {
                        {"1C", false},
                        {"Database", true},
                        {"Other health state", true}
                    };
                    this.Sender.Tell(new HealthCheckMessageResponse(healthResult));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}