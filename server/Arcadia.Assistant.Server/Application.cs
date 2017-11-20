namespace Arcadia.Assistant.Server
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;

    using Arcadia.Assistant.Organization;

    public class Application : IDisposable
    {
        public ActorSystem ActorSystem { get; private set; }

        public ServerActorsCollection ServerActors { get; private set; }

        public void Start()
        {
            var config = ConfigurationFactory.ParseString(
                @"
                akka {
                    actor {
                        provider: remote
                    }

                remote {
                    dot-netty.tcp {
                        port: 63301
                    }
                }
            ");

            this.ActorSystem = ActorSystem.Create("arcadia-assistant", config);
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