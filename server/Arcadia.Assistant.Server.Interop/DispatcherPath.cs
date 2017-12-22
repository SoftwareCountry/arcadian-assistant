namespace Arcadia.Assistant.Server.Interop
{
    public class DispatcherPath
    {
        private readonly ActorPathsBuilder pathBuilder;

        public DispatcherPath(ActorPathsBuilder pathBuilder)
        {
            this.pathBuilder = pathBuilder;
        }

        public const string DispatcherAgentName = "remote-dispatcher";

        public string Get()
        {
            return this.pathBuilder.Get($"/{DispatcherAgentName}");
        }
    }
}