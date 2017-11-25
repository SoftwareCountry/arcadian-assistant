namespace Arcadia.Assistant.Server.Console
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Configuration;

    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        private static readonly AutoResetEvent closing = new AutoResetEvent(false);

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

            using (var app = new Application(config))
            {
                app.Start();
                Console.CancelKeyPress += OnExit;
                closing.WaitOne();
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting...");
            closing.Set();
        }
    }
}
