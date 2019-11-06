namespace Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces
{
    public interface IDownloadApplicationSettings
    {
        string BuildsFolder { get; }

        int DownloadBuildIntervalMinutes { get; }

        string RenameBuildFilePattern { get; }

        string ApiToken { get; }

        string AndroidGetBuildsUrl { get; }

        string AndroidGetBuildDownloadLinkTemplateUrl { get; }

        string IosGetBuildsUrl { get; }

        string IosGetBuildDownloadLinkTemplateUrl { get; }
    }
}