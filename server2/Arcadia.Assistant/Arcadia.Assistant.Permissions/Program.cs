namespace Arcadia.Assistant.Permissions
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Employees.Contracts;

    using Logging;

    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Organization.Contracts;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<Permissions>("Arcadia.Assistant.PermissionsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new EmployeesModule());
                builder.RegisterModule(new OrganizationModule());
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Permissions).Name);

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