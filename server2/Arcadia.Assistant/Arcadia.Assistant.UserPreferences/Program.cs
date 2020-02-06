namespace Arcadia.Assistant.UserPreferences
{
    using System;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;
    using Logging;

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

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterActor<UserPreferencesStorage>();
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}