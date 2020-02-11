namespace Arcadia.Assistant.Inbox
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;
    using Arcadia.Assistant.Logging;
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Microsoft.Extensions.Logging;
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
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<Inbox>("Arcadia.Assistant.InboxType");
                builder.Register(x => new ImapConfiguration(configurationPackage.Settings.Sections["IMAP"])).AsSelf().SingleInstance();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.TryResolve(out ILogger val) ? val : null;
                logger?.LogInformation($"Service type '{typeof(Inbox).Name}' registered. Process: {Process.GetCurrentProcess().Id}.");
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