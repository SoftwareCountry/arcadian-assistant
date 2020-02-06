namespace Arcadia.Assistant.Web
{
    using System;
    using System.Diagnostics;
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
                ServiceRuntime.RegisterServiceAsync("Arcadia.Assistant.WebType",
                    context => new Web(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Web).Name);

                // Prevents this host process from terminating so services keeps running. 
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