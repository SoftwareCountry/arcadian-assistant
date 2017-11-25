namespace Arcadia.Assistant.Server
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;

    using Microsoft.Extensions.Configuration;

    public class Application : IDisposable
    {
        private readonly IConfigurationRoot config;

        public Application(IConfigurationRoot config)
        {
            this.config = config;
        }

        public ActorSystem ActorSystem { get; private set; }

        public ServerActorsCollection ServerActors { get; private set; }

        public void Start()
        {
            var akkaConfig = ConfigurationFactory.ParseString(this.config["akka"]);

            this.ActorSystem = ActorSystem.Create("arcadia-assistant", akkaConfig);
            var builder = new ActorSystemBuilder(this.ActorSystem);
            this.ServerActors = builder.AddRootActors();
        }

        public async Task Stop()
        {
            if (this.ActorSystem != null)
            {
                await this.ActorSystem.Terminate();
                this.ActorSystem.Dispose();
                this.ActorSystem = null;
                this.ServerActors = null;
            }
        }

        public void Dispose()
        {
            this.ActorSystem?.Dispose();
            this.ActorSystem = null;
            this.ServerActors = null;
        }
    }
}