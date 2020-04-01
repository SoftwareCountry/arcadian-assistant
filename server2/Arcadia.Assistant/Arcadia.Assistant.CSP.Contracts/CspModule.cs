namespace Arcadia.Assistant.CSP
{
    using System;

    using Autofac;

    using Contracts;

    using CspdbConnector.AzureBus;
    using CspdbConnector.Contracts;
    using CspdbConnector.Contracts.Configuration;

    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;

    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Logging;

    public class CspModule : Module
    {
        public CspModule(string connectionString)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ArcadiaCspContext>().AsSelf().InstancePerDependency();
            builder.RegisterType<CspdbAzureBusConnectorImpl>().As<ICspdbConnector>();
            builder.RegisterType<MassTransitBusService>().AsSelf().SingleInstance();
            builder.Register(x => new CspConfiguration
            {
                ConnectionString =
                    "Endpoint=sb://arcadia-cspdb-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=My3KYmJOIIgqo9fBP9DYxbqiNfIsAKI7AlrZWfQz38I=",
                CompanyId = 154, //TODO: config file
                HeadDepartmentAbbreviation = "GMG",
                UserIdentityDomain = "arcadia.spb.ru"
            }).SingleInstance();
            builder.RegisterType<CspEmployeeQuery>().InstancePerDependency();
            builder.RegisterType<CspDepartmentsQuery>().InstancePerDependency();
            builder.AddMassTransit(x =>
                // add the bus to the container
                x.AddBus(context => Bus.Factory.CreateUsingAzureServiceBus(busConfig =>
                {
                    try
                    {
                        var settingsLogger = context.Resolve<CspConfiguration>();
                        var host = busConfig.Host(settingsLogger.ConnectionString, hostConfig =>
                        {
                            hostConfig.TransportType = TransportType.AmqpWebSockets;
                        });

                        busConfig.DefaultMessageTimeToLive = TimeSpan.FromSeconds(20);
                        busConfig.EnableDeadLetteringOnMessageExpiration = false;
                        busConfig.PrefetchCount = 160;
                        busConfig.MaxConcurrentCalls = 32;
                        busConfig.EnableBatchedOperations = false;
                        busConfig.UseBsonSerializer();
                        busConfig.SelectBasicTier();

                        // Register all available endpoints using ClassName
                        busConfig.ConfigureEndpoints(context);
                    }
                    catch (Exception e)
                    {
                        var logger = context.ResolveOptional<ILogger>();
                        logger?.LogCritical(e, "MassTransit initialization error");
                        throw;
                    }
                })
                )
            );
            /*
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IEmailNotifications>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.EmailNotifications")));
                    */
        }
    }
}