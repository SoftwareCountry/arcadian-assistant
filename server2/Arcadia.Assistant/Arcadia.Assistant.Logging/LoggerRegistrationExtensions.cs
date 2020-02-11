namespace Arcadia.Assistant.Logging
{
    using System;
    using System.Fabric;

    using Autofac;

    using Microsoft.Extensions.Logging;

    using ServiceFabric.Logging;

    public static class LoggerRegistrationExtensions
    {
        public static void RegisterServiceLogging(
            this ContainerBuilder builder,
            LoggerSettings loggerSettings)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Register((x, p) =>
                {
                    var sc = p.TypedAs<ServiceContext>();
                    var loggerFactoryBuilder = new LoggerFactoryBuilder(sc);
                    return loggerFactoryBuilder.CreateLoggerFactory(loggerSettings.ApplicationInsightsKey);
                })
                .Named<ILoggerFactory>("servicefabriclogger")
                .SingleInstance();

            builder.Register(x =>
            {
                if (x.IsRegistered<ServiceContext>())
                {
                    var sc = x.Resolve<ServiceContext>();
                    var loggerFactory = x.ResolveNamed<ILoggerFactory>("servicefabriclogger", TypedParameter.From(sc));
                    return loggerFactory;
                }

                return new LoggerFactory();
            }).As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();
        }

        public static void RegisterServiceLogging(
            this ContainerBuilder builder,
            LoggerSettings loggerSettings,
            ServiceContext context)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var loggerFactoryBuilder = new LoggerFactoryBuilder(context);
            var loggerFactory = loggerFactoryBuilder.CreateLoggerFactory(loggerSettings.ApplicationInsightsKey);
            builder.Register(x => loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();
        }
    }
}