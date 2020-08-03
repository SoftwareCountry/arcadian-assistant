namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Assistant.Notifications.Contracts;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Configuration;

    using CSP;

    using Employees.Contracts;

    using Logging;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Notifications;

    using NotificationTemplates.Configuration;

    using Organization.Contracts;

    using Permissions.Contracts;

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
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var connectionString =
                    configurationPackage.Settings.Sections["Csp"].Parameters["ConnectionString"].Value;

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<SickLeaves>("Arcadia.Assistant.SickLeavesType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterType<SickLeaveCancellationStep>().AsSelf();
                builder.RegisterType<SickLeaveCreationStep>().AsSelf();
                builder.RegisterType<SickLeaveProlongationStep>().AsSelf();
                builder.RegisterModule(new CspModule(connectionString));
                builder.RegisterModule<PermissionsModule>();
                builder.RegisterModule<EmployeesModule>();
                builder.RegisterModule<OrganizationModule>();
                builder.RegisterModule<NotificationsModule>();

                builder.Register(x =>
                    NotificationConfigurationLoader.Load<ISickLeaveCreateNotificationConfiguration>(
                        configurationPackage.Settings.Sections[SickLeaveNotificationTemplate.SickLeaveCreated]));
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<ISickLeaveProlongNotificationConfiguration>(
                        configurationPackage.Settings.Sections[SickLeaveNotificationTemplate.SickLeaveProlonged]));
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<ISickLeaveCancelNotificationConfiguration>(
                        configurationPackage.Settings.Sections[SickLeaveNotificationTemplate.SickLeaveCancelled]));
                builder.RegisterType<SickLeaveChangeNotification>().SingleInstance().AsSelf();

                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<SickLeaves>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(SickLeaves).Name, Process.GetCurrentProcess().Id);
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