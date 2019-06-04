namespace Arcadia.Assistant.ApplicationBuilds
{
    public class SetApplicationBuildNumber
    {
        public SetApplicationBuildNumber(string applicationKey, int buildNumber)
        {
            this.ApplicationKey = applicationKey;
            this.BuildNumber = buildNumber;
        }

        public string ApplicationKey { get; }

        public int BuildNumber { get; }
    }
}