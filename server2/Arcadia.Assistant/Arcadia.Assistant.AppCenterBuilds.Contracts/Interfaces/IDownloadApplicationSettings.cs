namespace Arcadia.Assistant.AppCenterBuilds.Contracts
{
    public interface IDownloadApplicationSettings
    {
        int? DownloadBuildIntervalMinutes { get; }

        string? ApiToken { get; }

        string? AndroidGetBuildsUrl { get; }

        string? AndroidGetBuildDownloadLinkTemplateUrl { get; }

        string? IosGetBuildsUrl { get; }

        string? IosGetBuildDownloadLinkTemplateUrl { get; }
    }
}