namespace Arcadia.Assistant.Web.Download
{
    public class DownloadApplicationBuild
    {
        public static readonly DownloadApplicationBuild Instance = new DownloadApplicationBuild();

        public class Response
        {
            public static readonly Response Instance = new Response();

            public Response(string message = null)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}