namespace Arcadia.Assistant.Notifications
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using DeviceRegistry.Contracts;

    using Interfaces;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Models;

    using PushNotifications.Contracts;

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
                builder.Register(x => new NotificationSettings(configurationPackage.Settings.Sections["Notifications"])).As<INotificationSettings>().SingleInstance();
                builder.RegisterStatelessService<Notifications>("Arcadia.Assistant.NotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule<DeviceRegistryModule>();
                builder.RegisterModule<PushNotificationsModule>();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.TryResolve(out ILogger val) ? val : null;
                logger?.LogInformation($"Service type '{typeof(Notifications).Name}' registered. Process: {Process.GetCurrentProcess().Id}.");
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