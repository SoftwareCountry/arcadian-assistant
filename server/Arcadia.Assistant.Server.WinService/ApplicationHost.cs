namespace Arcadia.Assistant.Server.WinService
{
    using System.IO;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Configuration;

    using Microsoft.Extensions.Configuration;

    public class ApplicationHost
    {
        private readonly Application application;

        public ApplicationHost(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();

            var config = configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddHoconContent("akka.conf", "Akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            this.application = new MonitoredApplication(config);
        }

        public void Start()
        {
            this.application.Start();
        }

        public async Task Stop()
        {
            await this.application.Stop();
            this.application.Dispose();
        }
    }
}