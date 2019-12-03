namespace Arcadia.Assistant.Logging
{
    using System.Fabric;
    using System.Globalization;

    using ApplicationInsights;

    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.Logging;

    using PropertyMap;

    using Serilog;
    using Serilog.Context;

    using ServiceFabric.Remoting.CustomHeaders;

    /// <summary>
    ///     Implementation of <see cref="ILoggerFactoryBuilder" />
    /// </summary>
    public class LoggerFactoryBuilder : ILoggerFactoryBuilder
    {
        private readonly ServiceContext _context;

        /// <summary>
        ///     Create a new instance
        /// </summary>
        /// <param name="context">The <see cref="ServiceContext" /> of the service or actor to monitor</param>
        public LoggerFactoryBuilder(ServiceContext context)
        {
            this._context = context;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ILoggerFactory" /> that provides logging to application insights using SeriLog
        /// </summary>
        /// <param name="aiKey">The Application Insights key used for logging</param>
        /// <returns>An instance of <see cref="LoggerFactory" /></returns>
        public ILoggerFactory CreateLoggerFactory(string aiKey)
        {
            var configuration = new TelemetryConfiguration
            {
                InstrumentationKey = aiKey
            };

            configuration.TelemetryInitializers.Add(new OperationContextTelemetryInitializer(() =>
                RemotingContext.GetData(HeaderIdentifiers.TraceId)?.ToString()));

            new LiveStreamProvider(configuration).Enable();

            var loggerFactory = new LoggerFactory();
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .ApplicationInsights(
                    configuration,
                    (logEvent, formatter) => new TelemetryBuilder(this._context, logEvent).LogEventToTelemetryConverter())
                .CreateLogger();

            this.InitContextProperties();

            loggerFactory.AddSerilog(logger, true);

            return loggerFactory;
        }

        private void InitContextProperties()
        {
            LogContext.PushProperty(ServiceContextProperties.ServiceTypeName, this._context.ServiceTypeName);
            LogContext.PushProperty(ServiceContextProperties.ServiceName, this._context.ServiceName);
            LogContext.PushProperty(ServiceContextProperties.PartitionId, this._context.PartitionId);
            LogContext.PushProperty(ServiceContextProperties.NodeName, this._context.NodeContext.NodeName);
            LogContext.PushProperty(ServiceContextProperties.ApplicationName, this._context.CodePackageActivationContext.ApplicationName);
            LogContext.PushProperty(ServiceContextProperties.ApplicationTypeName, this._context.CodePackageActivationContext.ApplicationTypeName);
            LogContext.PushProperty(ServiceContextProperties.ServicePackageVersion, this._context.CodePackageActivationContext.CodePackageVersion);

            if (this._context is StatelessServiceContext)
            {
                LogContext.PushProperty(ServiceContextProperties.InstanceId, this._context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
            }
            else if (this._context is StatefulServiceContext)
            {
                LogContext.PushProperty(ServiceContextProperties.ReplicaId, this._context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}