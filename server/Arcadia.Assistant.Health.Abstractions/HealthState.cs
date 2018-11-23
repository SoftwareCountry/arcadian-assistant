namespace Arcadia.Assistant.Health.Abstractions
{
    public class HealthState
    {
        public HealthState(bool value, string details)
        {
            Value = value;
            Details = details;
        }

        public bool Value { get; }

        public string Details { get; }
    }
}