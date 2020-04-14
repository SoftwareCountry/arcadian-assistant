using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.Avatars.Manager
{
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Contracts;

    using CSP.WebApi.Contracts;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors.Client;
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
                var connectionString = configurationPackage.Settings.Sections["Csp"].Parameters["ConnectionString"].Value;

                var builder = new ContainerBuilder();

                builder.RegisterModule(new CspApiModule());
                builder.RegisterServiceFabricSupport();
                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new AvatarsModule());
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                builder.RegisterStatelessService<Manager>("Arcadia.Assistant.Avatars.ManagerType");

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<Manager>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.", typeof(Manager).Name, Process.GetCurrentProcess().Id);
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
