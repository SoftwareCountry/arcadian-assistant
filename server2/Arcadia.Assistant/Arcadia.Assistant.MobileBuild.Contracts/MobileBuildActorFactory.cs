namespace Arcadia.Assistant.MobileBuild.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class MobileBuildActorFactory : IMobileBuildActorFactory
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public MobileBuildActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IMobileBuildActor MobileBuild(string type)
        {
            var actor = this.actorProxyFactory.CreateActorProxy<IMobileBuildActor>(new ActorId($"file-{type}"),
                serviceName: "MobileBuildActorService");
            return actor;
        }
    }
}