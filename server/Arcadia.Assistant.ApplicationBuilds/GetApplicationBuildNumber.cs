namespace Arcadia.Assistant.ApplicationBuilds
{
    public class GetApplicationBuildNumber
    {
        public GetApplicationBuildNumber(string applicationKey)
        {
            this.ApplicationKey = applicationKey;
        }

        public string ApplicationKey { get; }

        public class Response
        {
            public Response(int? buildNumber)
            {
                this.BuildNumber = buildNumber;
            }

            public int? BuildNumber { get; }
        }
    }
}