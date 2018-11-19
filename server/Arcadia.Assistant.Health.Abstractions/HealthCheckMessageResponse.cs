namespace Arcadia.Assistant.Health.Abstractions
{
    using System.Collections.Generic;

    public class HealthCheckMessageResponse
    {
        public HealthCheckMessageResponse(IDictionary<string, HealthState> healthStates)
        {
            HealthStates = healthStates;
        }

        public IDictionary<string, HealthState> HealthStates { get; }
    }
}