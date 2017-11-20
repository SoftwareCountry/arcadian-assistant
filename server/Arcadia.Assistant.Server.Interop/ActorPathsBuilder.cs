namespace Arcadia.Assistant.Server.Interop
{
    using System;

    public class ActorPathsBuilder
    { 
        private readonly string remoteUserPath;

        public ActorPathsBuilder()
        {
            this.remoteUserPath = "/user";
        }

        public ActorPathsBuilder(string actorSystem, string address, int port)
        {
            var remotePath = $"akka.tcp://{actorSystem}@{address}:{port}";
            this.remoteUserPath = remotePath + "/user";
        }

        public string Get(string path)
        {
            if (!path.StartsWith("/", StringComparison.Ordinal))
            {
                path = string.Intern("/" + path);
            }

            return string.Intern(this.remoteUserPath + path);
        }
    }
}