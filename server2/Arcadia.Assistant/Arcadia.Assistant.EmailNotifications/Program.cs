namespace Arcadia.Assistant.EmailNotifications
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Logging;

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

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<EmailNotifications>("Arcadia.Assistant.EmailNotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.Register(x => new EmailNotificationSettings(configurationPackage.Settings.Sections["EmailNotification"])).AsSelf()
                    .SingleInstance();
                builder.Register(x => new SmtpSettings(configurationPackage.Settings.Sections["Smtp"])).AsSelf()
                    .SingleInstance();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<EmailNotifications>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(EmailNotifications).Name, Process.GetCurrentProcess().Id);
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