namespace Arcadia.Assistant.Web.Configuration
{
    public interface IDownloadApplicationSettings
    {
        string BuildsFolder { get; }

        string AndroidFilePattern { get; }

        string IosFilePattern { get; }
    }
}