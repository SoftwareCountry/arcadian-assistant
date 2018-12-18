namespace Arcadia.Assistant.Web.Configuration
{
    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        public string ApkDownloadPath { get; set; }

        public string IpaDownloadPath { get; set; }
    }
}