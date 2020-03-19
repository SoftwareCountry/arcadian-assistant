namespace Arcadia.Assistant.PushNotifications
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Interfaces;

    using Logging;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Models;

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

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.Register(x => new PushSettings(configurationPackage.Settings.Sections["PushNotifications"]))
                    .As<IPushSettings>().SingleInstance();
                builder.RegisterStatelessService<PushNotifications>("Arcadia.Assistant.PushNotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));
                builder.Populate(services);

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<PushNotifications>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(PushNotifications).Name, Process.GetCurrentProcess().Id);
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