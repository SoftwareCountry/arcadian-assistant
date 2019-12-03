namespace Arcadia.Assistant.Logging.ApplicationInsights
{
    using System;

    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;

    internal class OperationContextTelemetryInitializer : ITelemetryInitializer
    {
        private readonly Func<string> operationIdProvider;

        public OperationContextTelemetryInitializer(Func<string> operationIdProvider)
        {
            this.operationIdProvider = operationIdProvider;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Operation.Id = this.operationIdProvider.Invoke();
            telemetry.Context.Operation.ParentId = this.operationIdProvider.Invoke();

            if (telemetry.Context.Operation.Name == null)
            {
                telemetry.Context.Operation.Name = Guid.NewGuid().ToString();
            }
        }
    }
}