namespace Arcadia.Assistant.Server.Console
{
    using System;
    using System.IO;
    using System.Threading;

    using Arcadia.Assistant.Configuration;
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        private static readonly AutoResetEvent Closing = new AutoResetEvent(false);

        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();

            var config = configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddHoconContent("akka.conf", "Akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddUserSecrets<Program>()
                .Build();

            var settings = config.Get<AppSettings>();

            new AppInsightTelemetry(settings).Setup();

            using (var app = new Application(config))
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
