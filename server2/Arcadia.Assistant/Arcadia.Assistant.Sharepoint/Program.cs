namespace Arcadia.Assistant.Sharepoint
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using ExternalStorages.SharepointOnline;
    using ExternalStorages.SharepointOnline.Contracts;

    using Microsoft.Extensions.DependencyInjection;

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

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.Register(x => new SharepointOnlineConfiguration(configurationPackage.Settings.Sections["Sharepoint"])).As<ISharepointOnlineConfiguration>().SingleInstance();
                builder.Register(x => new SharepointDepartmentsCalendarsSettings(configurationPackage.Settings.Sections["DepartmentsCalendars"])).As<ISharepointDepartmentsCalendarsSettings>().SingleInstance();
                builder.RegisterStatelessService<Sharepoint>("Arcadia.Assistant.SharepointType");
                builder.Populate(services);

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Sharepoint).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}