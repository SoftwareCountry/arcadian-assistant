using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.AnniversaryFeed
{
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using CSP;

    using Logging;

    using Microsoft.Extensions.Logging;

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

                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<AnniversaryFeed>("Arcadia.Assistant.AnniversaryFeedType");
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger>();
                logger?.LogInformation($"Service type '{typeof(AnniversaryFeed).Name}' registered. Process: {Process.GetCurrentProcess().Id}.");
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
