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
            public static readonly Success Instance = new Success();
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