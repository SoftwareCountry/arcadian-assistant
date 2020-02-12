using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.Logging;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace Arcadia.Assistant.Avatars
{
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

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterActor<Avatar>();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger>();
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                logger?.LogCritical(e, e.Message);
                throw;
            }
        }
    }
}
