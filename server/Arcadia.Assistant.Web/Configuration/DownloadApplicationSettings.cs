namespace Arcadia.Assistant.Web.Configuration
{
    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        public string BuildsFolder { get; set; }

        public string AndroidFilePattern { get; set; }

        public string IosFilePattern { get; set; }
    }
}