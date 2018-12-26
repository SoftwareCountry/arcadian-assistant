namespace Arcadia.Assistant.Server
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.AutoFac;

    using Autofac;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Arcadia.Assistant.Configuration.Configuration;

    public class Application : IDisposable
    {
        protected readonly IConfigurationRoot Config;

        private IContainer container;

        private ActorSystem ActorSystem { get; set; }

        public Application(IConfigurationRoot config)
        {
            this.Config = config;
        }

        public ServerActorsCollection ServerActors { get; private set; }

        public void Start()
        {
            var akkaConfig = ConfigurationFactory.ParseString(this.Config["Akka"]);

            this.ActorSystem = ActorSystem.Create("arcadia-assistant", akkaConfig);
            this.OnStart(this.ActorSystem);

            var services = new ServiceCollection();
            services.AddHttpClient();

            var di = new DependencyInjection();

            this.container = di.GetContainer(this.Config, services);

            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(this.container, this.ActorSystem);

            var builder = new ActorSystemBuilder(this.ActorSystem);

            var appSettings = this.container.Resolve<AppSettings>();
            this.ServerActors = builder.AddRootActors(
                appSettings.Messaging.CalendarEventsMail,
                appSettings.Messaging.CalendarEventsPush,
                appSettings.Messaging.Imap);
        }

        protected virtual void OnStart(ActorSystem actorSystem)
        {
        }

        protected virtual void OnStop(ActorSystem actorSystem)
        {
        }

        public async Task Stop()
        {
            if (this.ActorSystem != null)
            {
                this.OnStop(this.ActorSystem);
                await this.ActorSystem.Terminate();
            }
        }

        public virtual void Dispose()
        {
            this.ActorSystem?.Dispose();
            this.container.Dispose();
            this.ActorSystem = null;
            this.ServerActors = null;
            this.container = null;
        }
    }
}