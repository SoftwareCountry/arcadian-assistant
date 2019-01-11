namespace Arcadia.Assistant.Web.Configuration
{
    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        public string BuildsFolder { get; set; }

        public int DownloadBuildIntervalMinutes { get; set; }

        public string RenameBuildFilePattern { get; set; }

        public string ApiToken { get; set; }

        public string AndroidGetBuildsUrl { get; set; }

        public string AndroidGetBuildDownloadLinkTemplateUrl { get; set; }

        public string IosGetBuildsUrl { get; set; }

        public string IosGetBuildDownloadLinkTemplateUrl { get; set; }

        public string IosManifestTemplateFileName { get; set; }
    }
}