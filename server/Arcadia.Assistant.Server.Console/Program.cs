namespace Arcadia.Assistant.Server.Console
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Configuration;

    using Autofac;

    using Microsoft.Extensions.Configuration;
    using Arcadia.Assistant.DI;

    internal class Program
    {
        private static readonly AutoResetEvent Closing = new AutoResetEvent(false);

        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var config = configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddHoconContent("akka.conf", "akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var container = new ContainerBuilder();

            container.RegisterModule(new DatabaseModule(config.GetConnectionString("ArcadiaCSP")));
            container.RegisterModule<OrganizationModule>();

            using (var app = new Application(config, container))
            {
                app.Start();
                Console.CancelKeyPress += OnExit;
                Closing.WaitOne();
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting...");
            Closing.Set();
        }
    }
}
