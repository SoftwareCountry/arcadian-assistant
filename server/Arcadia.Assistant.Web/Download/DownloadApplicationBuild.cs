namespace Arcadia.Assistant.Web.Download
{
    using System;

    public class DownloadApplicationBuild
    {
        public static readonly DownloadApplicationBuild Instance = new DownloadApplicationBuild();

        public class Response
        {
        }

        public class Success : Response
        {
            public Success(bool updateAvailable)
            {
                this.UpdateAvailable = updateAvailable;
            }

            public bool UpdateAvailable { get; }
        }

        public class Error : Response
        {
            public Error(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}