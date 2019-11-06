namespace Arcadia.Assistant.AppCenterBuilds.Contracts
{
    public class GetLatestApplicationBuildPath
    {
        public GetLatestApplicationBuildPath(ApplicationTypeEnum applicationTypeEnum)
        {
            this.ApplicationType = applicationTypeEnum;
        }

        public ApplicationTypeEnum ApplicationType { get; }

        public class Response
        {
            public Response(string path)
            {
                this.Path = path;
            }

            public string Path { get; }
        }
    }
}