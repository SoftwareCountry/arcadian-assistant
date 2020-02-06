namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using CSP;

    using Logging;

    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Runtime;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var connectionString = configurationPackage.Settings.Sections["Csp"].Parameters["ConnectionString"].Value;

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<SickLeaves>("Arcadia.Assistant.SickLeavesType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterType<SickLeaveCancellationStep>().AsSelf();
                builder.RegisterType<SickLeaveCreationStep>().AsSelf();
                builder.RegisterType<SickLeaveProlongationStep>().AsSelf();
                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SickLeaves).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}