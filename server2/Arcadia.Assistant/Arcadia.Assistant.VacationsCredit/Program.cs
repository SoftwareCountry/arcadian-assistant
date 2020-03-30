using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.VacationsCredit
{
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Inbox.Contracts;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            ILogger? logger = null;
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var inboxSection = configurationPackage.Settings.Sections["Inbox"];
                var inboxConfiguration = new InboxConfiguration(inboxSection);

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<VacationsCredit>("Arcadia.Assistant.VacationsCreditType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new InboxModule());
                builder.RegisterType<VacationsDaysEmailsLoader>().As<IVacationsDaysLoader>();
                builder.RegisterInstance(inboxConfiguration);
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<VacationsCredit>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.", typeof(VacationsCredit).Name, Process.GetCurrentProcess().Id);
                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                logger?.LogCritical(e, e.Message);
                throw;
            }
        }
    }
}
