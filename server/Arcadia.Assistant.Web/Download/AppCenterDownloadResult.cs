namespace Arcadia.Assistant.Web.Download
{
    public class AppCenterDownloadResult
    {
        public AppCenterDownloadResult(int buildNumber, string filePath)
        {
            this.BuildNumber = buildNumber;
            this.FilePath = filePath;
        }

        public int BuildNumber { get; }

        public string FilePath { get; }
    }
}