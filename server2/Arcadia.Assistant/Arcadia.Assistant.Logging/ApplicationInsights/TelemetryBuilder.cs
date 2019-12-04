namespace Arcadia.Assistant.Logging.ApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fabric;
    using System.Linq;

    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;

    using PropertyMap;

    using Serilog.Events;

    internal class TelemetryBuilder
    {
        private readonly ServiceContext context;
        private readonly LogEvent logEvent;

        public TelemetryBuilder(ServiceContext context, LogEvent logEvent)
        {
            this.context = context;
            this.logEvent = logEvent;
        }

        public ITelemetry LogEventToTelemetryConverter()
        {
            var serviceFabricEvent = ServiceFabricEvent.Undefined;
            if (this.logEvent.Properties.TryGetValue(SharedProperties.EventId, out var eventId))
            {
                int.TryParse(((StructureValue)eventId).Properties[0].Value.ToString(), out serviceFabricEvent);
            }

            ITelemetry telemetry;
            switch (serviceFabricEvent)
            {
                case ServiceFabricEvent.Exception:
                    telemetry = this.CreateExceptionTelemetry();
                    break;
                case ServiceFabricEvent.ApiRequest:
                    telemetry = this.CreateRequestTelemetry();
                    break;
                case ServiceFabricEvent.Metric:
                    telemetry = this.CreateMetricTelemetry();
                    break;
                case ServiceFabricEvent.ServiceRequest:
                case ServiceFabricEvent.Dependency:
                    telemetry = this.CreateDependencyTelemetry();
                    break;
                default:
                    telemetry = this.CreateTraceTelemetry();
                    break;
            }

            this.SetContextProperties(telemetry);

            return telemetry;
        }

        private ITelemetry CreateRequestTelemetry()
        {
            var requestTelemetry = new RequestTelemetry
            {
                ResponseCode = this.TryGetStringValue(ApiRequestProperties.StatusCode),
                Url = new Uri($"{this.TryGetStringValue(ApiRequestProperties.Scheme)}://{this.TryGetStringValue(ApiRequestProperties.Host)}{this.TryGetStringValue(ApiRequestProperties.Path)}"),
                Name = $"{this.TryGetStringValue(ApiRequestProperties.Method)} {this.TryGetStringValue(ApiRequestProperties.Path)}",
                Timestamp = DateTime.Parse(this.TryGetStringValue(ApiRequestProperties.StartTime)),
                Duration = TimeSpan.FromMilliseconds(double.Parse(this.TryGetStringValue(ApiRequestProperties.DurationInMs))),
                Success = bool.Parse(this.TryGetStringValue(ApiRequestProperties.Success)),
                Properties =
                {
                    { ApiRequestProperties.Method, this.TryGetStringValue(ApiRequestProperties.Method) },
                    { ApiRequestProperties.Headers, this.TryGetStringValue(ApiRequestProperties.Headers) },
                    { ApiRequestProperties.Body, this.TryGetStringValue(ApiRequestProperties.Body) }
                }
            };

            requestTelemetry.Context.Operation.Name = requestTelemetry.Name;
            requestTelemetry.Id = this.TryGetStringValue(SharedProperties.TraceId);

            this.AddLogEventProperties(requestTelemetry, typeof(ApiRequestProperties).GetFields().Select(f => f.GetRawConstantValue().ToString()));

            return requestTelemetry;
        }

        private ITelemetry CreateDependencyTelemetry()
        {
            var dependencyTelemetry = new DependencyTelemetry
            {
                Name = this.TryGetStringValue(DependencyProperties.DependencyTypeName),
                Duration = TimeSpan.FromMilliseconds(double.Parse(this.TryGetStringValue(DependencyProperties.DurationInMs))),
                Data = this.TryGetStringValue(DependencyProperties.Name),
                Success = bool.Parse(this.TryGetStringValue(DependencyProperties.Success)),
                Type = this.TryGetStringValue(DependencyProperties.Type),
                Timestamp = DateTime.Parse(this.TryGetStringValue(DependencyProperties.StartTime))
            };

            dependencyTelemetry.Id = dependencyTelemetry.Data;
            dependencyTelemetry.Context.Operation.Name = dependencyTelemetry.Name;

            this.AddLogEventProperties(dependencyTelemetry, typeof(DependencyProperties).GetFields().Select(f => f.GetRawConstantValue().ToString()));

            return dependencyTelemetry;
        }

        private ITelemetry CreateMetricTelemetry()
        {
            var metricTelemetry = new MetricTelemetry
            {
                Name = this.TryGetStringValue(MetricProperties.Name),
                Sum = double.Parse(this.TryGetStringValue(MetricProperties.Value)),
                Timestamp = this.logEvent.Timestamp
            };

            if (this.logEvent.Properties.TryGetValue(MetricProperties.MinValue, out var min))
            {
                metricTelemetry.Min = double.Parse(min.ToString());
            }

            if (this.logEvent.Properties.TryGetValue(MetricProperties.MaxValue, out var max))
            {
                metricTelemetry.Max = double.Parse(max.ToString());
            }

            this.AddLogEventProperties(metricTelemetry, typeof(MetricProperties).GetFields().Select(f => f.GetRawConstantValue().ToString()));

            return metricTelemetry;
        }

        private ITelemetry CreateExceptionTelemetry()
        {
            var exceptionTelemetry = new ExceptionTelemetry(this.logEvent.Exception)
            {
                SeverityLevel = this.logEvent.Level.ToSeverityLevel(),
                Timestamp = this.logEvent.Timestamp
            };

            this.AddLogEventProperties(exceptionTelemetry);

            return exceptionTelemetry;
        }

        private ITelemetry CreateTraceTelemetry()
        {
            var traceTelemetry = new TraceTelemetry(this.logEvent.RenderMessage())
            {
                SeverityLevel = this.logEvent.Level.ToSeverityLevel(),
                Timestamp = this.logEvent.Timestamp
            };

            this.AddLogEventProperties(traceTelemetry);

            return traceTelemetry;
        }

        private void SetContextProperties(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = FabricEnvironmentVariable.ServicePackageName;
            telemetry.Context.Cloud.RoleInstance = FabricEnvironmentVariable.ServicePackageActivationId ?? FabricEnvironmentVariable.ServicePackageInstanceId;
            telemetry.Context.Component.Version = this.context.CodePackageActivationContext.CodePackageVersion;

            if (!telemetry.Context.Properties.ContainsKey(ServiceContextProperties.NodeName))
            {
                if (!string.IsNullOrEmpty(FabricEnvironmentVariable.NodeName))
                {
                    telemetry.Context.Properties.Add(ServiceContextProperties.NodeName, FabricEnvironmentVariable.NodeName);
                }
            }

#if Debug
            telemetry.Context.Operation.SyntheticSource = "DebugSession";
#else
            if (Debugger.IsAttached)
            {
                telemetry.Context.Operation.SyntheticSource = "DebuggerAttached";
            }
#endif

            if (this.logEvent.Properties.TryGetValue(SharedProperties.TraceId, out var value))
            {
                var id = ((ScalarValue)value).Value.ToString();
                telemetry.Context.Operation.ParentId = id;
                telemetry.Context.Operation.Id = id;
            }
        }

        private void AddLogEventProperties(ISupportProperties telemetry, IEnumerable<string>? excludePropertyKeys = null)
        {
            var excludedPropertyKeys = new List<string>
            {
                ServiceContextProperties.NodeName,
                ServiceContextProperties.ServicePackageVersion
            };

            if (excludePropertyKeys != null)
            {
                excludedPropertyKeys.AddRange(excludePropertyKeys);
            }

            foreach (var property in this.logEvent
                .Properties
                .Where(property => property.Value != null && !excludedPropertyKeys.Contains(property.Key) && !telemetry.Properties.ContainsKey(property.Key)))
            {
                ApplicationInsightsPropertyFormatter.WriteValue(property.Key, property.Value, telemetry.Properties);
            }
        }

        private string TryGetStringValue(string propertyName)
        {
            if (!this.logEvent.Properties.TryGetValue(propertyName, out var value))
            {
                throw new ArgumentException($"LogEvent does not contain required property {propertyName} for EventId {this.logEvent.Properties[SharedProperties.EventId]}", propertyName);
            }

            return ((ScalarValue)value).Value.ToString();
        }
    }
}