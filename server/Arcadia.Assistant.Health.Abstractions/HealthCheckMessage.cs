namespace Arcadia.Assistant.Health.Abstractions
{
    public class HealthCheckMessage
    {
        public HealthCheckMessage(HealthCheckType checkType)
        {
            CheckType = checkType;
        }

        public HealthCheckType CheckType { get; }
    }
}