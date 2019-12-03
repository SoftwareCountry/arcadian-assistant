namespace Arcadia.Assistant.AppCenterBuilds
{
    using System.Fabric.Description;

    using Contracts;

    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        private const int DefaultDownloadBuildIntervalMinutes = 720;

        public DownloadApplicationSettings(ConfigurationSection configurationSection)
        {
            this.ApiToken = configurationSection.Parameters["ApiToken"].Value;
            this.DownloadBuildIntervalMinutes = int.TryParse(configurationSection.Parameters["DownloadBuildIntervalMinutes"].Value, out var interval) 
                ? interval 
                : DefaultDownloadBuildIntervalMinutes;
            this.AndroidGetBuildsUrl = configurationSection.Parameters["AndroidGetBuildsUrl"].Value;
            this.AndroidGetBuildDownloadLinkTemplateUrl = configurationSection.Parameters["AndroidGetBuildDownloadLinkTemplateUrl"].Value;
            this.IosGetBuildsUrl = configurationSection.Parameters["IosGetBuildsUrl"].Value;
            this.IosGetBuildDownloadLinkTemplateUrl = configurationSection.Parameters["IosGetBuildDownloadLinkTemplateUrl"].Value;
        }

        public int DownloadBuildIntervalMinutes { get; set; }

        public string ApiToken { get; set; }

        public string AndroidGetBuildsUrl { get; set; }

        public string AndroidGetBuildDownloadLinkTemplateUrl { get; set; }

        public string IosGetBuildsUrl { get; set; }

        public string IosGetBuildDownloadLinkTemplateUrl { get; set; }
    }
}