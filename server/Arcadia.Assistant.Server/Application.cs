namespace Arcadia.Assistant.Server
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;

    using Autofac;
    using Calendar.SickLeave;
    using Feeds;
    using Health.Abstractions;
    using Helpdesk;
    using Interop;
    using Microsoft.Extensions.Configuration;
    using Organization;
    using UserPreferences;

    public class Application : IDisposable
    {
        protected readonly IConfigurationRoot Config;

        private IContainer container;

        private ActorSystem ActorSystem { get; set; }

        public Application(IConfigurationRoot config)
        {
            this.Config = config;
        }

        public void Start()
        {
            var akkaConfig = ConfigurationFactory.ParseString(this.Config["Akka"]);

            this.ActorSystem = ActorSystem.Create("arcadia-assistant", akkaConfig);
            this.OnStart(this.ActorSystem);

            var di = new DependencyInjection();

            this.container = di.GetContainer(this.Config);

            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(this.container, this.ActorSystem);

            CreateRootActors();
        }

        public async Task Stop()
        {
            if (this.ActorSystem != null)
            {
                this.OnStop(this.ActorSystem);
                await this.ActorSystem.Terminate();
                this.Dispose();
            }
        }

        public virtual void Dispose()
        {
            this.ActorSystem?.Dispose();
            this.container.Dispose();
            this.ActorSystem = null;
            this.container = null;
        }

        protected virtual void OnStart(ActorSystem actorSystem)
        {
        }

        protected virtual void OnStop(ActorSystem actorSystem)
        {
        }

        private void CreateRootActors()
        {
            var departments = this.ActorSystem.ActorOf(this.ActorSystem.DI().Props<OrganizationActor>(), WellKnownActorPaths.Organization);
            this.ActorSystem.ActorOf(this.ActorSystem.DI().Props<HealthChecker>(), WellKnownActorPaths.Health);
            this.ActorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            this.ActorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(departments)), WellKnownActorPaths.SharedFeeds);
            this.ActorSystem.ActorOf(this.ActorSystem.DI().Props<UserPreferencesActor>(), WellKnownActorPaths.UserPreferences);

            this.ActorSystem.ActorOf(this.ActorSystem.DI().Props<SendEmailSickLeaveActor>(), "send-sick-leave-email");
        }
    }
}