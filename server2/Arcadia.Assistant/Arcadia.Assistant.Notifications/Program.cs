namespace Arcadia.Assistant.Notifications
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using DeviceRegistry.Contracts;

    using EmailNotifications.Contracts;

    using Employees.Contracts;

    using Interfaces;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Models;

    using PushNotifications.Contracts;

    using UserPreferences.Contracts;

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
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.Register(x =>
                    {
                        var configLogger = x.Resolve<ILogger<NotificationSettings>>();
                        return new NotificationSettings(configurationPackage.Settings.Sections["Notifications"],
                            configLogger);
                    }
                ).As<INotificationSettings>().SingleInstance();
                builder.RegisterStatelessService<Notifications>("Arcadia.Assistant.NotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule<EmployeesModule>();
                builder.RegisterModule<UsersPreferencesModule>();
                builder.RegisterModule<DeviceRegistryModule>();
                builder.RegisterModule<PushNotificationsModule>();
                builder.RegisterModule<EmailNotificationsModule>();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<Notifications>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(Notifications).Name, Process.GetCurrentProcess().Id);

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