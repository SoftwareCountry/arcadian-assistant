namespace Arcadia.Assistant.AppCenterBuilds
{
    using System.Fabric.Description;

    using Contracts;

    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        public DownloadApplicationSettings(ConfigurationSection configurationSection)
        {
            this.ApiToken = configurationSection.Parameters["ApiToken"].Value;
            this.DownloadBuildIntervalMinutes = int.Parse(configurationSection.Parameters["DownloadBuildIntervalMinutes"].Value);
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