namespace Arcadia.Assistant.WorkHoursCredit
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.NotificationTemplates.Configuration;
    using Arcadia.Assistant.Organization.Contracts;
    using Arcadia.Assistant.WorkHoursCredit.Notification;
    using Autofac;
    using Autofac.Integration.ServiceFabric;

    using Logging;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Debug;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Model;

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
                builder.RegisterStatelessService<WorkHoursCredit>("Arcadia.Assistant.WorkHoursCreditType");
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                builder.Register((c) =>
                {
                    var opt = new DbContextOptionsBuilder<WorkHoursCreditContext>();
                    opt
                        .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider(), }))
                        .UseInMemoryDatabase("workhours");
                    return opt.Options;
                }).SingleInstance();

                builder.RegisterModule<EmployeesModule>();
                builder.RegisterModule<OrganizationModule>();
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<IWorkHoursCreditCreateNotificationConfiguration>(
                        configurationPackage.Settings.Sections[WorkHoursCreditNotificationTemplate.WorkHoursCreditCreated]));
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<IWorkHoursCreditApproveNotificationConfiguration>(
                        configurationPackage.Settings.Sections[WorkHoursCreditNotificationTemplate.WorkHoursCreditApproved]));
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<IWorkHoursCreditApproveRequireNotificationConfiguration>(
                        configurationPackage.Settings.Sections[WorkHoursCreditNotificationTemplate.WorkHoursCreditApproveRequire]));
                builder.Register(x =>
                    NotificationConfigurationLoader.Load<IWorkHoursCreditCancelNotificationConfiguration>(
                        configurationPackage.Settings.Sections[WorkHoursCreditNotificationTemplate.WorkHoursCreditCancelled]));
                builder.RegisterType<WorkHoursCreditNotification>().SingleInstance().AsSelf();
                builder.Register(c => new WorkHoursCreditContext(c.Resolve<DbContextOptions<WorkHoursCreditContext>>())).AsSelf();

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<WorkHoursCredit>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.", typeof(WorkHoursCredit).Name, Process.GetCurrentProcess().Id);
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