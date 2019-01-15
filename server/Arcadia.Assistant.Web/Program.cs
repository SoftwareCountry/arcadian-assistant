namespace Arcadia.Assistant.Web
{
    using System;

    using Autofac.Extensions.DependencyInjection;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using NLog.Web;

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting up...");
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "The program failed to start");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureServices(services => services.AddAutofac())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}
