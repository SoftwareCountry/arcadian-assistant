namespace Arcadia.Assistant.Server.WinService
{
    using System.ComponentModel;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }
    }
}
