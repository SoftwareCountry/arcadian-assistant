namespace Arcadia.Assistant.Sharepoint
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Linq;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Employees.Contracts;

    using ExternalStorages.Abstractions;
    using ExternalStorages.SharepointOnline;
    using ExternalStorages.SharepointOnline.Contracts;

    using Logging;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Models;

    using Organization.Contracts;

    using SickLeaves.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

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
                var sharepointSection = configurationPackage.Settings.Sections["Sharepoint"];
                var calendarEventIdField = sharepointSection.Parameters.TryGetValue("CalendarEventIdField", out var val)
                    ? val.Value
                    : null;

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.Register(x => new SharepointOnlineConfiguration(sharepointSection))
                    .As<ISharepointOnlineConfiguration>().SingleInstance();
                builder.Register(x =>
                        new SharepointSynchronizationSettings(configurationPackage.Settings.Sections["Service"]))
                    .As<ISharepointSynchronizationSettings>().SingleInstance();
                builder.Register(x =>
                        new SharepointDepartmentsCalendarsSettings(
                            configurationPackage.Settings.Sections["DepartmentsCalendars"]))
                    .As<ISharepointDepartmentsCalendarsSettings>().SingleInstance();
                builder
                    .Register(ctx =>
                    {
                        if (string.IsNullOrEmpty(calendarEventIdField))
                        {
                            return new SharepointFieldsMapper();
                        }

                        var mapping = SharepointFieldsMapper.DefaultMapping
                            .Union(new[]
                            {
                                SharepointFieldsMapper.CreateMapping(x => x.CalendarEventId, calendarEventIdField)
                            });
                        return new SharepointFieldsMapper(mapping.ToArray());
                    })
                    .As<ISharepointFieldsMapper>();

                builder.RegisterType<SharepointRequestExecutor>().As<ISharepointRequestExecutor>();
                builder.RegisterType<SharepointAuthTokenService>().As<ISharepointAuthTokenService>();
                builder.RegisterType<SharepointConditionsCompiler>().As<ISharepointConditionsCompiler>();
                builder.RegisterType<SharepointStorage>().As<IExternalStorage>();
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterStatelessService<Sharepoint>("Arcadia.Assistant.SharepointType");
                builder.RegisterModule<VacationsModule>();
                builder.RegisterModule<WorkHoursCreditModule>();
                builder.RegisterModule<SickLeavesModule>();
                builder.RegisterModule<EmployeesModule>();
                builder.RegisterModule<OrganizationModule>();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));
                builder.Populate(services);

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<Sharepoint>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(Sharepoint).Name, Process.GetCurrentProcess().Id);
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