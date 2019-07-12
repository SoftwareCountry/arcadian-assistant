namespace Arcadia.Assistant.Inbox
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Microsoft.ServiceFabric.Services.Runtime;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var imapSection = configurationPackage.Settings.Sections["IMAP"];
                var imapConfiguration = new ImapConfiguration(imapSection);

                ServiceRuntime.RegisterServiceAsync("Arcadia.Assistant.InboxType",
                    context => new Inbox(context, imapConfiguration)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Inbox).Name);

                // Prevents this host process from terminating so services keep running.
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