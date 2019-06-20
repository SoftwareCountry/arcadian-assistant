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

    using CSP;

    using Microsoft.ServiceFabric.Actors.Client;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
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

                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterServiceFabricSupport();
                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterModule(new AvatarsModule());

                builder.RegisterStatelessService<Manager>("Arcadia.Assistant.Avatars.ManagerType");

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Manager).Name);
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
