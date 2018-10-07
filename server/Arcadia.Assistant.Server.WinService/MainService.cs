namespace Arcadia.Assistant.Server.WinService
{
    using System.IO;
    using System.ServiceProcess;

    using Arcadia.Assistant.Configuration;
    using Arcadia.Assistant.Configuration.Configuration;

    using Microsoft.Extensions.Configuration;

    public partial class MainService : ServiceBase
    {
        private Application application;

        public MainService()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();

            var config = configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddHoconContent("akka.conf", "Akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var settings = config.Get<AppSettings>();

            new AppInsightTelemetry().Setup(settings);

            this.application = new MonitoredApplication(config);
            this.application.Start();
        }

        protected override void OnStop()
        {
            this.application?.Stop().Wait();
        }
    }
}
