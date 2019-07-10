namespace Arcadia.Assistant.WorkHoursCredit
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Autofac;
    using Autofac.Integration.ServiceFabric;

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
            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<WorkHoursCredit>("Arcadia.Assistant.WorkHoursCreditType");


                builder.Register((c) =>
                {
                    var opt = new DbContextOptionsBuilder<WorkHoursCreditContext>();
                    opt
                        .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider(), }))
                        .UseInMemoryDatabase("workhours");
                    return opt.Options;
                }).SingleInstance();

                builder.Register(c => new WorkHoursCreditContext(c.Resolve<DbContextOptions<WorkHoursCreditContext>>())).AsSelf();

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(WorkHoursCredit).Name);

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