namespace Arcadia.Assistant.Web.Download.AppCenter
{
    using System;

    public class AppCenterBuildDownloadModel
    {
        public string Uri { get; set; }

        public DateTimeOffset BuildDate { get; set; }
    }
}