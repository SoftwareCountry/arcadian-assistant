namespace Arcadia.Assistant.Web.Configuration
{
    public interface IDownloadApplicationSettings
    {
        string ApkDownloadPath { get; }

        string IpaDownloadPath { get; }
    }
}