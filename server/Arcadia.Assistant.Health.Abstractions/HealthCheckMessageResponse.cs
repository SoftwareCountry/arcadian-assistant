namespace Arcadia.Assistant.Health.Abstractions
{
    using System.Collections.Generic;

    public class HealthCheckMessageResponse
    {
        public HealthCheckMessageResponse(IDictionary<string, bool> healthState)
        {
            HealthState = healthState;
        }

        public IDictionary<string, bool> HealthState { get; }
    }
}