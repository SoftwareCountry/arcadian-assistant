namespace Arcadia.Assistant.Server
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;

    using Autofac;

    using Microsoft.Extensions.Configuration;

    public class Application : IDisposable
    {
        private readonly IConfigurationRoot config;

        private IContainer container;

        public Application(IConfigurationRoot config)
        {
            this.config = config;
        }

        public ActorSystem ActorSystem { get; private set; }

        public ServerActorsCollection ServerActors { get; private set; }

        public void Start()
        {
            var akkaConfig = ConfigurationFactory.ParseString(this.config["Akka"]);

            this.ActorSystem = ActorSystem.Create("arcadia-assistant", akkaConfig);

            var di = new DependencyInjection();

            this.container = di.GetContainer(this.config);

            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(this.container, this.ActorSystem);

            var builder = new ActorSystemBuilder(this.ActorSystem);
            this.ServerActors = builder.AddRootActors();
        }

        public async Task Stop()
        {
            if (this.ActorSystem != null)
            {
                await this.ActorSystem.Terminate();
                this.Dispose();
            }
        }

        public void Dispose()
        {
            this.ActorSystem?.Dispose();
            this.container.Dispose();
            this.ActorSystem = null;
            this.ServerActors = null;
            this.container = null;
        }
    }
}