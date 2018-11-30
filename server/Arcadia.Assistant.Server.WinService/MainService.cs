namespace Arcadia.Assistant.Server.WinService
{
    using System.ServiceProcess;

    public partial class MainService : ServiceBase
    {
        private ApplicationHost host;

        public MainService()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.host = new ApplicationHost(args);
        }

        protected override void OnStop()
        {
            this.host?.Stop().Wait();
        }
    }
}
