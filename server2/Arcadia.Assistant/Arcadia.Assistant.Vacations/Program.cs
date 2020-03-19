namespace Arcadia.Assistant.Vacations
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;
    using Arcadia.Assistant.Logging;
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using CSP;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Runtime;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
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
                var connectionString = configurationPackage.Settings.Sections["Csp"].Parameters["ConnectionString"].Value;
                var updateInterval = TimeSpan.FromMinutes(double.Parse(configurationPackage.Settings.Sections["Service"].Parameters["UpdateIntervalMinutes"].Value));

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<Vacations>("Arcadia.Assistant.VacationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.Register(x => new Settings() { ChangesCheckInterval = updateInterval }).SingleInstance();
                builder.RegisterType<VacationsStorage>().SingleInstance();
                builder.RegisterType<VacationChangesWatcher>().SingleInstance();
                builder.RegisterType<VacationChangesCheck>().SingleInstance();
                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<Vacations>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.", typeof(Vacations).Name, Process.GetCurrentProcess().Id);
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