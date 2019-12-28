namespace Arcadia.Assistant.Web
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;
    using Arcadia.Assistant.Logging;
    using Microsoft.ServiceFabric.Services.Runtime;
    using ServiceFabric.Logging;

    using Microsoft.Extensions.Logging;
    using ServiceFabric.Logging.Extensions;

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
                var loggerSettings = new LoggerSettings(configurationPackage.Settings.Sections["Logging"]);

                ServiceRuntime.RegisterServiceAsync("Arcadia.Assistant.WebType",
                    context =>
                    {
                        var loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory(loggerSettings.ApplicationInsightsKey);
                        var logger = loggerFactory.CreateLogger<Web>();

                        return new Web(context, logger);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Web).Name);

                // Prevents this host process from terminating so services keeps running. 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}