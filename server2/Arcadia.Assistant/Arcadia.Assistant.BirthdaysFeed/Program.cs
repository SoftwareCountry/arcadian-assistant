using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.BirthdaysFeed
{
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using CSP;

    using Employees.Contracts;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors.Client;

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
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var connectionString = configurationPackage.Settings.Sections["Csp"].Parameters["ConnectionString"].Value;

                var builder = new ContainerBuilder();

                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<BirthdaysFeed>("Arcadia.Assistant.BirthdaysFeedType");
                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterModule(new EmployeesModule());
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger>();
                logger?.LogInformation($"Service type '{typeof(BirthdaysFeed).Name}' registered. Process: {Process.GetCurrentProcess().Id}.");
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
