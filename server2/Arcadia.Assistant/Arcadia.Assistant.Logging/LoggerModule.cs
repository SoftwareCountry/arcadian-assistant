namespace Arcadia.Assistant.Logging
{
    using Autofac;
    using Microsoft.Extensions.Logging;
    using ServiceFabric.Logging;
    using System.Fabric;

    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((x, p) =>
                {
                    var sc = p.TypedAs<ServiceContext>();
                    var loggerFactoryBuilder = new LoggerFactoryBuilder(sc);
                    var settings = x.Resolve<LoggerSettings>();
                    return loggerFactoryBuilder.CreateLoggerFactory(settings.ApplicationInsightsKey);
                })
                .Named<ILoggerFactory>("servicefabriclogger")
                .SingleInstance();

            builder.Register(x =>
            {
                if (x.IsRegistered<ServiceContext>())
                {
                    var sc = x.Resolve<ServiceContext>();
                    return x.ResolveNamed<ILoggerFactory>("servicefabriclogger", new TypedParameter(typeof(ServiceContext), sc));
                }

                return new LoggerFactory();
            }).As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();
        }
    }
}